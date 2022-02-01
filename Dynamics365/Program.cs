﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics365
{
   public partial class Program
    {
        //How many records to create with this sample.
        private static readonly int numberOfRecords = 10000;

        [STAThread] // Added to support UX
        static void Main(string[] args)
        {
            CrmServiceClient service = Connect.GetOrganizationServiceClientSecret(
                 "95d26c6d-9ff7-4092-9c57-18a0950f3d0a",
                 "GCK7Q~Ccd7yB6H3S2~61vDrC5JbS6nIaCg~Ur",
                 "https://org4e0b44fb.crm8.dynamics.com/");


            //Entity acc = new Entity("account");
            //acc["name"] = "Zeest";
            //var createacc = service.Create(acc);
            //Console.WriteLine("Account created!");

            #region Optimize Connection settings

            //Change max connections from .NET to a remote service default: 2
            System.Net.ServicePointManager.DefaultConnectionLimit = 65000;
            //Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4
            System.Threading.ThreadPool.SetMinThreads(100, 100);
            //Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server
            System.Net.ServicePointManager.Expect100Continue = false;
            //Can decreas overall transmission overhead but can cause delay in data packet arrival
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

            #endregion Optimize Connection settings


            try
            {
               
                if (service.IsReady)
                {
                    #region Sample Code

                    ////////////////////////////////////

                    #region Set up

                    

                    #endregion Set up

                    #region Demonstrate



                    // Generate a list of account entities to create.

                    var accountsToImport = new List<Entity>();
                    var count = 0;
                    Console.WriteLine($"Preparing to create {numberOfRecords} acccount records");
                    while (count < numberOfRecords)
                    {
                        var account = new Entity("account");
                        account["name"] = $"Account {count}";
                        accountsToImport.Add(account);
                        count++;
                    }

                    try
                    {
                        Console.WriteLine($"Creating {accountsToImport.Count} accounts");

                        var startCreate = DateTime.Now;

                        //Import the list of accounts
                        var createdAccounts = CreateEntities(service, accountsToImport);

                        var secondsToCreate = (DateTime.Now - startCreate).TotalSeconds;

                        Console.WriteLine($"Created {accountsToImport.Count} accounts in  {Math.Round(secondsToCreate)} seconds.");

                        Console.WriteLine("Press Enter to continue");
                        Console.Read();
                        Console.WriteLine($"Deleting {createdAccounts.Count} accounts");

                        var startDelete = DateTime.Now;

                        //Delete the list of accounts created
                        DeleteEntities(service, createdAccounts.ToList());

                        var secondsToDelete = (DateTime.Now - startDelete).TotalSeconds;

                        Console.WriteLine($"Deleted {createdAccounts.Count} accounts in {Math.Round(secondsToDelete)} seconds.");
                    }
                    catch (AggregateException)
                    {
                        // Handle exceptions
                    }

                    Console.WriteLine("Done.");
                    Console.ReadLine();
                }

                #endregion Demonstrate

                #endregion Sample Code

                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
               // SampleHelpers.HandleException(ex);
            }
            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }






        }
    }
}
