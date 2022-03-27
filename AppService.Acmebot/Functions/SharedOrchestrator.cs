using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AppService.Acmebot.Models;

using DurableTask.TypedProxy;

using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppService.Acmebot.Functions
{
    public class SharedOrchestrator
    {
        [FunctionName(nameof(IssueCertificate))]
        public async Task<Certificate> IssueCertificate([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var (site, dnsNames, forceDns01Challenge) = context.GetInput<(Site, string[], bool)>();

            var activity = context.CreateActivityProxy<ISharedActivity>();

            // Always using DNS Auth for Container Apps
            await activity.Dns01Precondition(dnsNames);


            // 新しく ACME Order を作成する
            var orderDetails = await activity.Order(dnsNames);

            // 既に確認済みの場合は Challenge をスキップする
            if (orderDetails.Payload.Status != "ready")
            {
                // 複数の Authorizations を処理する
                IReadOnlyList<AcmeChallengeResult> challengeResults;

                // ACME Challenge を実行
                challengeResults = await activity.Dns01Authorization(orderDetails.Payload.Authorizations);

                // DNS レコードの変更が伝搬するまで 10 秒遅延させる
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(10), CancellationToken.None);

                // Azure DNS で正しくレコードが引けるか確認
                await activity.CheckDnsChallenge(challengeResults);


                // ACME Answer を実行
                await activity.AnswerChallenges(challengeResults);

                // Order のステータスが ready になるまで 60 秒待機
                await activity.CheckIsReady((orderDetails, challengeResults));

                // 作成した DNS レコードを削除
                await activity.CleanupDnsChallenge(challengeResults);

            }

            // CSR を作成し Finalize を実行
            var (finalize, rsaParameters) = await activity.FinalizeOrder((dnsNames, orderDetails));

            // Finalize の時点でステータスが valid の時点はスキップ
            if (finalize.Payload.Status != "valid")
            {
                // Finalize 後のステータスが valid になるまで 60 秒待機
                finalize = await activity.CheckIsValid(finalize);
            }

            // 証明書をダウンロードし App Service へアップロード
            var certificate = await activity.UploadCertificate((site, dnsNames[0], forceDns01Challenge, finalize, rsaParameters));

            return certificate;
        }
    }
}
