using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Configuration;
using Microsoft.Xrm.Sdk;

namespace Dynamics365
{
    public partial class Program
    {
            /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        ///

        /// <summary>
        /// Creates entities in parallel
        /// </summary>
        /// <param name="svc">The CrmServiceClient instance to use</param>
        /// <param name="entities">A List of entities to create.</param>
        /// <returns></returns>
        private static ConcurrentBag<EntityReference> CreateEntities(CrmServiceClient svc, List<Entity> entities)
        {
            var createdEntityReferences = new ConcurrentBag<EntityReference>();

            Parallel.ForEach(entities,
                new ParallelOptions() { MaxDegreeOfParallelism = svc.RecommendedDegreesOfParallelism },
                () =>
                {
                    //Clone the CrmServiceClient for each thread
                    return svc.Clone();
                },
                (entity, loopState, index, threadLocalSvc) =>
                {
                    // In each thread, create entities and add them to the ConcurrentBag
                    // as EntityReferences
                    createdEntityReferences.Add(
                        new EntityReference(
                            entity.LogicalName,
                            threadLocalSvc.Create(entity)
                            )
                        );

                    return threadLocalSvc;
                },
                (threadLocalSvc) =>
                {
                    //Dispose the cloned CrmServiceClient instance
                    if (threadLocalSvc != null)
                    {
                        threadLocalSvc.Dispose();
                    }
                });

            //Return the ConcurrentBag of EntityReferences
            return createdEntityReferences;
        }

        /// <summary>
        /// Deletes a list of entity references
        /// </summary>
        /// <param name="svc">The CrmServiceClient instance to use</param>
        /// <param name="entityReferences">A List of entity references to delete.</param>
        private static void DeleteEntities(CrmServiceClient svc, List<EntityReference> entityReferences)
        {
            Parallel.ForEach(entityReferences,
                new ParallelOptions() { MaxDegreeOfParallelism = svc.RecommendedDegreesOfParallelism },
                () =>
                {
                    //Clone the CrmServiceClient for each thread
                    return svc.Clone();
                },
                (er, loopState, index, threadLocalSvc) =>
                {
                    // In each thread, delete the entities
                    threadLocalSvc.Delete(er.LogicalName, er.Id);

                    return threadLocalSvc;
                },
                (threadLocalSvc) =>
                {
                    //Dispose the cloned CrmServiceClient instance
                    if (threadLocalSvc != null)
                    {
                        threadLocalSvc.Dispose();
                    }
                });
        }

        /// <summary>
        /// Gets web service connection information from the app.config file.
        /// If there is more than one available, the user is prompted to select
        /// the desired connection configuration by name.
        /// </summary>
        /// <returns>A string containing web service connection configuration information.</returns>
       
    }
}

