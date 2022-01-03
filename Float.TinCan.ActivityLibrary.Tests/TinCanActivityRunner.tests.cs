using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Float.TinCan.ActivityLibrary.Definition;
using Float.TinCan.LocalLRSServer;
using TinCan;
using TinCan.Documents;
using TinCan.LRSResponses;
using Xunit;

namespace Float.TinCan.ActivityLibrary.Tests
{
    public class TinCanActivityRunnerTests
    {
        [Fact]
        public void TestContextActivities()
        {
            var activity = new StubActivity();
            var lrs = new StubLrs();
            var actor = new Agent
            {
                mbox = "user@example.com",
            };
            var serverDelegate = new StubDelegate();
            var runner = new TinCanActivityRunner(activity, lrs, actor, serverDelegate);
            runner.Run();
        }

        class StubLrs : ILRS
        {
            public Task<AboutLRSResponse> About()
            {
                return Task.FromResult(new AboutLRSResponse());
            }

            public Task<LRSResponse> ClearState(Activity activity, Agent agent, Guid? registration = null)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<LRSResponse> DeleteActivityProfile(ActivityProfileDocument profile)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<LRSResponse> DeleteAgentProfile(AgentProfileDocument profile)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<LRSResponse> DeleteState(StateDocument state)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<LRSResponse> ForceSaveAgentProfile(AgentProfileDocument profile)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<StatementsResultLRSResponse> MoreStatements(StatementsResult statementsResult)
            {
                return Task.FromResult(new StatementsResultLRSResponse());
            }

            public Task<StatementsResultLRSResponse> QueryStatements(StatementsQuery query)
            {
                return Task.FromResult(new StatementsResultLRSResponse());
            }

            public Task<ActivityProfileLRSResponse> RetrieveActivityProfile(string id, Activity activity)
            {
                return Task.FromResult(new ActivityProfileLRSResponse());
            }

            public Task<ProfileKeysLRSResponse> RetrieveActivityProfileIds(Activity activity)
            {
                return Task.FromResult(new ProfileKeysLRSResponse());
            }

            public Task<AgentProfileLRSResponse> RetrieveAgentProfile(string id, Agent agent)
            {
                return Task.FromResult(new AgentProfileLRSResponse());
            }

            public Task<ProfileKeysLRSResponse> RetrieveAgentProfileIds(Agent agent)
            {
                return Task.FromResult(new ProfileKeysLRSResponse());
            }

            public Task<StateLRSResponse> RetrieveState(string id, Activity activity, Agent agent, Guid? registration = null)
            {
                return Task.FromResult(new StateLRSResponse());
            }

            public Task<ProfileKeysLRSResponse> RetrieveStateIds(Activity activity, Agent agent, Guid? registration = null)
            {
                return Task.FromResult(new ProfileKeysLRSResponse());
            }

            public Task<StatementLRSResponse> RetrieveStatement(Guid id)
            {
                return Task.FromResult(new StatementLRSResponse());
            }

            public Task<StatementLRSResponse> RetrieveVoidedStatement(Guid id)
            {
                return Task.FromResult(new StatementLRSResponse());
            }

            public Task<LRSResponse> SaveActivityProfile(ActivityProfileDocument profile)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<LRSResponse> SaveAgentProfile(AgentProfileDocument profile)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<LRSResponse> SaveState(StateDocument state)
            {
                return Task.FromResult(new LRSResponse());
            }

            public Task<StatementLRSResponse> SaveStatement(Statement statement)
            {
                return Task.FromResult(new StatementLRSResponse());
            }

            public Task<StatementsResultLRSResponse> SaveStatements(List<Statement> statements)
            {
                return Task.FromResult(new StatementsResultLRSResponse());
            }

            public Task<StatementLRSResponse> VoidStatement(Guid id, Agent agent)
            {
                return Task.FromResult(new StatementLRSResponse());
            }
        }

        class StubActivity : IActivity
        {
            public string UUID => Guid.NewGuid().ToString();

            public string Name => "Stub Activity";

            public string Description => "This is a stub activity.";

            public IFile Thumbnail => null;

            public string Section => "Stub section";

            public string ActivityType => "activity_type";

            public string Keywords => "keywords";

            public IEnumerable<IAudience> Audiences => Enumerable.Empty<IAudience>();

            public IEnumerable<IFile> Files => Enumerable.Empty<IFile>();

            public IActivityGroup ActivityGroup => new StubGroup();

            public IActivityMetaData MetaData => null;

            public DateTimeOffset? LastFileModificationTime => null;

            public DateTimeOffset? CompletionDate { get; set; }

            public DateTimeOffset? NewUntilDate => null;

            public DateTimeOffset? LastUpdatedDate => null;

            public IEnumerable<IPointOfInterest> PointsOfInterest => Enumerable.Empty<IPointOfInterest>();

            public Uri TinCanActivityId => new Uri("http://example.com/id");

            public Uri TinCanActivityType => new Uri("http://example.com/type");

            public ContentStatus ContentCompletableStatus => ContentStatus.Other;

            public Uri ContentUri => new Uri("http://example.com");

            public double PercentComplete { get; set; }

            public bool IsComplete => false;

            public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }
        }

        class StubGroup : IActivityGroup
        {
            public string UUID => Guid.NewGuid().ToString();

            public string Name => "Stub Group";

            public string Keywords => "group keywords";

            public int Weight => 0;

            public IFile Thumbnail => null;

            public IEnumerable<IAudience> Audiences => Enumerable.Empty<IAudience>();

            public IEnumerable<IActivity> Activities => Enumerable.Empty<IActivity>();

            public IPackage Package => null;

            public IActivityGroup ParentActivityGroup => null;

            public IEnumerable<IActivityGroup> ChildActivityGroups => Enumerable.Empty<IActivityGroup>();

            public IEnumerable<ILink> Links => Enumerable.Empty<ILink>();

            public int NumNewActivities => 1;

            public Uri TinCanActivityId => new Uri("http://example.com/group/id");

            public Uri TinCanActivityType => new Uri("http://example.com/group/type");

            public ContentStatus ContentCompletableStatus => ContentStatus.Other;

            public Uri ContentUri => new Uri("http://example.com/content");

            public bool IsCertificateAvailable => false;

            public double PercentComplete { get; set; }

            public bool IsComplete => false;

            public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }
        }

        class StubDelegate : ILRSServerDelegate
        {
            public AgentProfileDocument AgentProfileDocumentForProfileId(string profileId)
            {
                throw new NotImplementedException();
            }

            public string GetAccessConrolAllowOrigin()
            {
                throw new NotImplementedException();
            }
        }
    }
}
