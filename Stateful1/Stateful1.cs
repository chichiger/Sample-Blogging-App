using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using AppCommon;
using Microsoft.ServiceFabric.Data;
using System.Collections;

namespace Stateful1
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    /// 

    internal sealed class Stateful1 : StatefulService, IMyService
    {
        private bool collectionsReady; // use this when primary is called
        public string loggedIn = "false";
        string[] logged = new string[] { "false" };
        List<string> results1 = new List<string>();
        private CancellationToken token;
        public Stateful1(StatefulServiceContext context)
            : base(context)
        {
        }


        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>


        //protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        //{
        //    //OLD CODE
        //    //return new ServiceReplicaListener[0];


        //}


        public async Task<string> NewPost(String t, String tag)

        {
            // prevent user from posting by checking if they are logged in or not
            if (logged[0] == "false")
            {
                return ("You must be logged in to post");
            }
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            //IReliableDictionary<string, string> MD = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("MD");
            IReliableDictionary<string, string> MD;
            ConditionalValue<IReliableDictionary<string, string>> MDResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("MD");
            if (MDResult.HasValue == true)
            {
                MD = MDResult.Value; // type reliable dictionary
            }
            else
            {
                // crash the process
                string causeofFailure = "Dictionary bug somewhere. Should be true";
                Environment.FailFast(causeofFailure);
                //return;
                //throw new Exception("Dictionary bug somewhere. Should be true");
                return ("Dictionary bug somewhere. Should be true"); // unreachable
            }

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                //CancellationTokenSource source = new CancellationTokenSource();
                //CancellationToken token = source.Token;
                // make dictionary for each hashtag

                IList<string> results = new List<string>();
                //string addResult = await MD.AddOrUpdateAsync(tx, tag, t, (key, value) => t);
                var hashTagDictionaryName = await MD.GetOrAddAsync(tx, tag, tag);

                // create a dictionary for each hashtag
                IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                //string addResult = await hashTagDictionary.AddOrUpdateAsync(tx, DateTime.Now.ToString(), t, (key, value) => t);
                bool addResult = await hashTagDictionary.TryAddAsync(tx, logged[0] + " " + DateTime.UtcNow.ToString(), t);

                // create enumerator to get the values from the dictionary
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await hashTagDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(token))
                {
                    results.Add(enumerator.Current.Key);
                    results.Add(enumerator.Current.Value);
                    results.Add("\n");
                }

                await tx.CommitAsync();

                foreach (string i in results)
                {
                    Console.WriteLine("{0}\t", i);
                }
                string output = string.Join(" ", results.ToArray());
                return "Successfully posted";

            }

        }

        public async Task<string> getPost(string tag)
        {
            IReliableDictionary<string, string> MD;
            IList<string> results = new List<string>();
            ConditionalValue<IReliableDictionary<string, string>> MDResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("MD");
            if (MDResult.HasValue == true)
            {
                MD = MDResult.Value; // type reliable dictionary
            }
            else
            {
                // crash the process
                string causeofFailure = "Dictionary bug somewhere. Should be true";
                Environment.FailFast(causeofFailure);
                throw new Exception("Dictionary bug somewhere. Should be true");
            }

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                ConditionalValue<string> HD = await MD.TryGetValueAsync(tx, tag);
                if (HD.HasValue == false)
                {
                    return "No posts with this hashtag";
                }
                var hashTagDictionaryName = await MD.GetOrAddAsync(tx, tag, tag);
                //ConditionalValue<IReliableDictionary<string, string>> HD = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>> (tag);

                IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);

                //IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await hashTagDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(token))
                {

                    results.Add(enumerator.Current.Key);
                    results.Add(enumerator.Current.Value);
                    //results.Add("\n");
                }

                await tx.CommitAsync();
                string[] convert = results.ToArray();
                string answer = "";
                for (int i = 0; i < convert.Length; i++)
                {
                    if (i % 2 == 0 && i != 0)
                    {
                        answer += "\n";
                        answer += convert[i].ToString();
                        answer += "~";
                    }
                    else
                    {
                        answer += convert[i].ToString();
                        answer += "~";
                    }
                }
                return answer;
                //string output = string.Join("\n", results.ToArray());
                //return output;
            }
        }


        public async Task<string> signUp(String username, String password)

        {

            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            IReliableDictionary<string, string> signD = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("signD");

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                IList<string> results = new List<string>();
                bool addUser = await signD.TryAddAsync(tx, username, password);

                if (addUser == false)
                {
                    return "Username already taken. Please enter a different one";
                }

                // create enumerator to get the values from the dictionary
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await signD.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(token))
                {
                    results.Add(enumerator.Current.Key);
                    results.Add(enumerator.Current.Value);
                    results.Add("\n");
                }
                await tx.CommitAsync();
                return "You are now signed up";
            }

        }

        public async Task<string> logIn(string username, string password)
        {
            IReliableDictionary<string, string> signD = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("signD");
            IList<string> results = new List<string>();


            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                ConditionalValue<string> userCombo = await signD.TryGetValueAsync(tx, username);
                await tx.CommitAsync();
                if (userCombo.HasValue == false)
                {
                    return "Username/Password incorrect. No such username exists";
                }
                else
                {

                    if (userCombo.Value == password)
                    {
                        loggedIn = username;
                        logged[0] = username;
                        return "Login successful";

                    }
                    else
                    {
                        return "Wrong username/password combination";
                    }
                }

            }
        }

        /// <summary>
        /// In NewImage, the parameters are the URL of the image link and the hashtag associated
        /// with this image. These two values are passed from the stateless front end to the stateful
        /// back end. The Master Dictionary will first check if this hashtag exists, and if it does, then you
        /// get the dictionary. If it doesn't exist, you create it. Then, inside that dictionary, you add
        /// the image URL. 
        /// </summary>
        public async Task<string> NewImage(String URL, String tag)

        {
            // prevent user from posting if they are not logged in
            if (logged[0] == "false")
            {
                return ("You must be logged in to post");
            }
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            IReliableDictionary<string, string> MD;
            ConditionalValue<IReliableDictionary<string, string>> MDResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("MD");
            if (MDResult.HasValue == true)
            {
                MD = MDResult.Value; // type reliable dictionary
            }
            else
            {
                // crash the process
                string causeofFailure = "Dictionary bug somewhere. Should be true";
                Environment.FailFast(causeofFailure);
                return ("Dictionary bug somewhere. Should be true"); // unreachable
            }

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                IList<string> results = new List<string>();
                var hashTagDictionaryName = await MD.GetOrAddAsync(tx, tag, tag);

                // create a dictionary for each hashtag
                IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                bool addResult = await hashTagDictionary.TryAddAsync(tx, logged[0] + " " + DateTime.UtcNow.ToString(), URL);

                await tx.CommitAsync();

                return "Successfully posted";
            }

        }

        public async Task<string> logOut()
        {

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                await tx.CommitAsync();
                logged[0] = "false";
                return "Logged Out";

            }
        }

        // just like the newImage, but instead of taking in a URL of the image you convert jpg to base 64 string
        public async Task<string> uploadImage(string URL, string tag)
        {
            // prevent user from posting if they are not logged in
            if (logged[0] == "false")
            {
                return ("You must be logged in to post");
            }
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            IReliableDictionary<string, string> MD;
            ConditionalValue<IReliableDictionary<string, string>> MDResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("MD");
            if (MDResult.HasValue == true)
            {
                MD = MDResult.Value; // type reliable dictionary
            }
            else
            {
                // crash the process
                string causeofFailure = "Dictionary bug somewhere. Should be true";
                Environment.FailFast(causeofFailure);
                return ("Dictionary bug somewhere. Should be true"); // unreachable
            }

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                IList<string> results = new List<string>();
                var hashTagDictionaryName = await MD.GetOrAddAsync(tx, tag, tag);

                // create a dictionary for each hashtag
                IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName + "Image");
                bool addResult = await hashTagDictionary.TryAddAsync(tx, logged[0] + " " + DateTime.UtcNow.ToString(), URL);
                await tx.CommitAsync();
                return "Successfully posted";

            }

        }

        public async Task<string> searchUp(string name)
        {

            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            IReliableDictionary<string, string> MD;
            ConditionalValue<IReliableDictionary<string, string>> MDResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("MD");
            if (MDResult.HasValue == true)
            {
                MD = MDResult.Value; // type reliable dictionary
            }
            else
            {
                // crash the process
                string causeofFailure = "Dictionary bug somewhere. Should be true";
                Environment.FailFast(causeofFailure);
                return ("Dictionary bug somewhere. Should be true"); // unreachable
            }

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                IList<string> results = new List<string>();

                // create enumerator to get the values from the dictionary
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await MD.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(token))
                {
                    //Console.WriteLine(enumerator.Current.Key , enumerator.Current.Value);
                    results.Add(enumerator.Current.Key);
                }
                await tx.CommitAsync();
                string[] options = results.ToArray();
                results1.Clear();
                foreach (string i in options)
                {
                    if (i.StartsWith(name))
                    {
                        results1.Add(i);
                    }

                }
                string output = string.Join(" , ", results1.ToArray());
                return output;

            }

        }


        public async Task<string> getImage(string tag)
        {
            IReliableDictionary<string, string> MD;
            IList<string> results = new List<string>();
            ConditionalValue<IReliableDictionary<string, string>> MDResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("MD");
            if (MDResult.HasValue == true)
            {
                MD = MDResult.Value; // type reliable dictionary
            }
            else
            {
                // crash the process
                string causeofFailure = "Dictionary bug somewhere. Should be true";
                Environment.FailFast(causeofFailure);
                throw new Exception("Dictionary bug somewhere. Should be true");
            }

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                ConditionalValue<string> HD = await MD.TryGetValueAsync(tx, tag);
                if (HD.HasValue == false)
                {
                    return "No posts with this hashtag";
                }
                var hashTagDictionaryName = await MD.GetOrAddAsync(tx, tag, tag);
                //ConditionalValue<IReliableDictionary<string, string>> HD = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>> (tag + "Image");

                IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName + "Image");

                //IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await hashTagDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(token))
                {

                    results.Add(enumerator.Current.Key);
                    results.Add(enumerator.Current.Value);
                }

                await tx.CommitAsync();
                string[] convert = results.ToArray();
                string answer = "";

                for (int i = 0; i < convert.Length; i++)
                {


                    answer += convert[i].ToString();
                    answer += "~";
                }


                return answer;
            }
        }


        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceCommunicationListener, name: "ServiceEndpoint") };
        }

        private ICommunicationListener CreateServiceCommunicationListener(ServiceContext context)
        {
            return new FabricTransportServiceRemotingListener(context, this);

        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // method executes when replica of your service becomes primary. the MD dictionary will always be created
            this.token = cancellationToken; // use this for enumerate
            await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("MD");
            this.collectionsReady = true;

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }


    }
}