// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Stateful1
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AppCommon;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    /// 
    internal sealed class Stateful1 : StatefulService, IMyService
    {
        public string LoggedIn = "false";
        private bool collectionsReady; // use this when primary is called
        private string[] logged = {"false"}; // variable for checking if user is logged in or not
        private List<string> results1 = new List<string>();
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

        

        /// <summary>
        /// This method gets the text or image from the user and the hashtag associated with it.
        /// During the transaction, a master dictionary will have the names of all the hashtags,
        /// and then the value of the dictionary will be the hashtag dictionary for that specific
        /// hashtag. The specific hashtag will have the username and date as the key, and the value
        /// is the value. getOrAddAsync is used to see if the hashtag dictionary is already created and if
        /// it's not, then it will be created and the post is stored inside. 
        /// </summary>
        public async Task<string> NewPost(string t, string tag)

        {
            // prevent user from posting by checking if they are logged in or not
            if (this.logged[0] == "false")
            {
                return ("You must be logged in to post");
            }
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            
            IReliableDictionary<string, string> md;
            ConditionalValue<IReliableDictionary<string, string>> mdResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("md");
            if (mdResult.HasValue)
            {
                md = mdResult.Value; // type reliable dictionary
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
                //CancellationTokenSource source = new CancellationTokenSource();
                //CancellationToken token = source.Token;
                // make dictionary for each hashtag
                string[] strArrayOne = new string[] { "" };
                strArrayOne = tag.Split(',');
                for (int i = 0; i < strArrayOne.Length; i++)
                {
                    //string addResult = await MD.AddOrUpdateAsync(tx, tag, t, (key, value) => t);
                    //string hashTagDictionaryName = await md.GetOrAddAsync(tx, tag, tag);
                    string hashTagDictionaryName = await md.GetOrAddAsync(tx, strArrayOne[i], strArrayOne[i]);
                    // create a dictionary for each hashtag
                    IReliableDictionary<string, string> hashTagDictionary =
                        await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                    //string addResult = await hashTagDictionary.AddOrUpdateAsync(tx, DateTime.Now.ToString(), t, (key, value) => t);
                    bool addResult = await hashTagDictionary.TryAddAsync(tx, this.logged[0] + " " + DateTime.UtcNow, t);
                }
                await tx.CommitAsync();

               
                return "Successfully posted";
            }
        }

        /// <summary>
        /// This method returns all of the posts with the hashtag requested by the user
        ///
        /// </summary>

        public async Task<string> getPost(string tag)
        {
            IReliableDictionary<string, string> md;
            IList<string> results = new List<string>();
            ConditionalValue<IReliableDictionary<string, string>> mdResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("md");
            // assert testing
            if (mdResult.HasValue)
            {
                md = mdResult.Value; // type reliable dictionary
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
                ConditionalValue<string> hd = await md.TryGetValueAsync(tx, tag);
                if (hd.HasValue == false)
                {
                    return "No posts with this hashtag";
                }
                string hashTagDictionaryName = await md.GetOrAddAsync(tx, tag, tag);
                //ConditionalValue<IReliableDictionary<string, string>> HD = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>> (tag);

                IReliableDictionary<string, string> hashTagDictionary =
                    await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);

                //IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await hashTagDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(this.token))
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


        public async Task<string> signUp(string username, string password)

        {
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            IReliableDictionary<string, string> signD = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("signD");

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {

                // should never be null because of javascript checking
                if (username == null || password == null)
                {
                    return "Please enter all fields";
                }
                bool addUser = await signD.TryAddAsync(tx, username, password);

                if (addUser == false)
                {
                    return "Username already taken. Please enter a different one";
                }

               
                await tx.CommitAsync();
                return "You are now signed up";
            }
        }

        /// <summary>
        /// Takes in a string username and string password and checks the
        /// dictionary for that combination. If the username is not in the dictionary
        /// it will return an error. If it is there, it will check the value to make sure
        /// it's the same and will say if it is successful or not
        /// </summary>
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
                        this.LoggedIn = username;
                        this.logged[0] = username;
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
        /// the image URL as the value 
        /// </summary>
        public async Task<string> NewImage(string url, string tag)

        {
            // prevent user from posting if they are not logged in
            if (this.logged[0] == "false")
            {
                return ("You must be logged in to post");
            }
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            // check to make sure the input is a URL
            if (!url.StartsWith("http://") || !url.StartsWith("https://") )
            {
                return "Not a valid URL link";
            }

            IReliableDictionary<string, string> md;
            ConditionalValue<IReliableDictionary<string, string>> mdResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("md");
            if (mdResult.HasValue)
            {
                md = mdResult.Value; // type reliable dictionary
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
                string[] strArrayOne = new string[] { "" };
                strArrayOne = tag.Split(',');
                for (int i = 0; i < strArrayOne.Length; i++)
                {
                    string hashTagDictionaryName = await md.GetOrAddAsync(tx, strArrayOne[i], strArrayOne[i]);

                    // create a dictionary for each hashtag
                    IReliableDictionary<string, string> hashTagDictionary =
                    await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                    bool addResult = await hashTagDictionary.TryAddAsync(tx, this.logged[0] + " " + DateTime.UtcNow.ToString(), url);
                }
               

                await tx.CommitAsync();

                return "Successfully posted";
            }
        }

        /// <summary>
        /// set the logged variable to false to indicate that you are logged out
        /// </summary>
        public async Task<string> logOut()
        {
            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                await tx.CommitAsync();
                this.logged[0] = "false";
                return "Logged Out";
            }
        }

        /// <summary>
        /// In upLoadImage, the parameters are the image as a 64 bit string and the hashtag associated
        /// with this image. These two values are passed from the stateless front end to the stateful
        /// back end. The Master Dictionary will first check if this hashtag exists, and if it does, then you
        /// get the dictionary. If it doesn't exist, you create it. Then, inside that dictionary, you add
        /// the image string as the value 
        /// </summary>
        public async Task<string> uploadImage(string url, string tag)
        {
            // prevent user from posting if they are not logged in
            if (this.logged[0] == "false")
            {
                return ("You must be logged in to post");
            }
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }

            IReliableDictionary<string, string> md;
            ConditionalValue<IReliableDictionary<string, string>> mdResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("md");
            if (mdResult.HasValue)
            {
                md = mdResult.Value; // type reliable dictionary
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
                string[] strArrayOne = new string[] { "" };
                strArrayOne = tag.Split(',');
                for (int i = 0; i < strArrayOne.Length; i++)
                {
                    string hashTagDictionaryName = await md.GetOrAddAsync(tx, strArrayOne[i], strArrayOne[i]);

                    // create a dictionary for each hashtag
                    IReliableDictionary<string, string> hashTagDictionary =
                        await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName + "Image");
                    bool addResult = await hashTagDictionary.TryAddAsync(tx, this.logged[0] + " " + DateTime.UtcNow, url);
                }
                   
                await tx.CommitAsync();
                return "Successfully posted";
            }
        }

        /// <summary>
        /// Whenever the user starts typing a letter, this function is triggered. It will take whatever the using is typing
        /// and loop through the master dictionary to see if there are any hashtags that start with the letter/phrase. If no
        /// matches are found, it will return a message saying that
        /// </summary>
        public async Task<string> searchUp(string name)
        {
            while (!this.collectionsReady)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            IReliableDictionary<string, string> md;
            ConditionalValue<IReliableDictionary<string, string>> mdResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("md");
            if (mdResult.HasValue)
            {
                md = mdResult.Value; // type reliable dictionary
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
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await md.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(this.token))
                {
                    //Console.WriteLine(enumerator.Current.Key , enumerator.Current.Value);
                    results.Add(enumerator.Current.Key);
                }
                await tx.CommitAsync();
                string[] options = results.ToArray();
                this.results1.Clear();
                foreach (string i in options)
                {
                    if (i.StartsWith(name))
                    {
                        this.results1.Add(i);
                    }
                }
                string output = string.Join(" , ", this.results1.ToArray());
                return output;
            }
        }

        /// <summary>
        /// returns image to user 
        /// </summary>
        public async Task<string> getImage(string tag)
        {
            IReliableDictionary<string, string> md;
            IList<string> results = new List<string>();
            ConditionalValue<IReliableDictionary<string, string>> mdResult = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>>("md");
            if (mdResult.HasValue)
            {
                md = mdResult.Value; // type reliable dictionary
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
                ConditionalValue<string> hd = await md.TryGetValueAsync(tx, tag);
                if (hd.HasValue == false)
                {
                    return "No posts with this hashtag";
                }
                string hashTagDictionaryName = await md.GetOrAddAsync(tx, tag, tag);
                //ConditionalValue<IReliableDictionary<string, string>> HD = await this.StateManager.TryGetAsync<IReliableDictionary<string, string>> (tag + "Image");

                IReliableDictionary<string, string> hashTagDictionary =
                    await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName + "Image");

                //IReliableDictionary<string, string> hashTagDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(hashTagDictionaryName);
                IAsyncEnumerator<KeyValuePair<string, string>> enumerator = (await hashTagDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(this.token))
                {
                    results.Add(enumerator.Current.Key);
                    results.Add(enumerator.Current.Value);
                }

                await tx.CommitAsync();
                string[] convert = results.ToArray();
                string answer = "";

                for (int i = 0; i < convert.Length; i++)
                {
                    answer += convert[i];
                    answer += "~";
                }

                return answer;
            }
        }


        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {new ServiceReplicaListener(this.CreateServiceCommunicationListener, name: "ServiceEndpoint")};
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
            await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("md");
            this.collectionsReady = true;

            IReliableDictionary<string, long> myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (ITransaction tx = this.StateManager.CreateTransaction())
                {
                    ConditionalValue<long> result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(
                        this.Context,
                        "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private ICommunicationListener CreateServiceCommunicationListener(ServiceContext context)
        {
            return new FabricTransportServiceRemotingListener(context, this);
        }
    }
}