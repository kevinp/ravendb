﻿using System;
using System.Net.Http;
using Raven.Client.Documents.Conventions;
using Raven.Client.Http;
using Raven.Client.Json.Converters;
using Sparrow.Json;

namespace Raven.Client.Server.Operations
{
    public class GetOngoingTaskInfoOperation : IServerOperation<OngoingTask>
    {
        private readonly string _database;
        private readonly long _taskId;
        private readonly OngoingTaskType _type;

        public GetOngoingTaskInfoOperation(string database, long taskId, OngoingTaskType type)
        {
            MultiDatabase.AssertValidName(database);
            _database = database;
            _taskId = taskId;
            _type = type;
        }

        public RavenCommand<OngoingTask> GetCommand(DocumentConventions conventions, JsonOperationContext context)
        {
            return new GetOngoingTaskInfoCommand(conventions, context, _database, _taskId, _type);
        }

        private class GetOngoingTaskInfoCommand : RavenCommand<OngoingTask>
        {
            private readonly JsonOperationContext _context;
            private readonly DocumentConventions _conventions;
            private readonly string _databaseName;
            private readonly long _taskId;
            private readonly OngoingTaskType _type;

            public GetOngoingTaskInfoCommand(
                DocumentConventions conventions,
                JsonOperationContext context,
                string database,
                long taskId,
                OngoingTaskType type
            )
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
                _databaseName = database ?? throw new ArgumentNullException(nameof(database));
                _taskId = taskId;
                _type = type;
            }

            public override HttpRequestMessage CreateRequest(ServerNode node, out string url)
            {
                url = $"{node.Url}/databases/{_databaseName}/task?key={_taskId}&type={_type}";

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get
                };

                return request;
            }

            public override void SetResponse(BlittableJsonReaderObject response, bool fromCache)
            {
                if (response != null)
                {
                    switch (_type)
                    {
                        case OngoingTaskType.Replication:
                            Result = JsonDeserializationClient.GetOngoingTaskReplicationResult(response);
                            break;
                        case OngoingTaskType.RavenEtl:
                            Result = JsonDeserializationClient.GetOngoingTaskRavenEtlResult(response);
                            break;
                        case OngoingTaskType.SqlEtl:
                            Result = JsonDeserializationClient.GetOngoingTaskSqlEtlResult(response);
                            break;
                        case OngoingTaskType.Backup:
                            Result = JsonDeserializationClient.GetOngoingTaskBackupResult(response);
                            break;
                        case OngoingTaskType.Subscription:
                            Result = JsonDeserializationClient.GetOngoingTaskSubscriptionResult(response);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public override bool IsReadRequest => false;
        }
    }

}
