using Newtonsoft.Json;

namespace NotificationsBot.Models.AzureModels.Release
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Artifact
    {
        public string sourceId { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public DefinitionReference definitionReference { get; set; }
        public bool isPrimary { get; set; }
        public bool isRetained { get; set; }
    }

    public class ArtifactsDownloadInput
    {
        public List<object> downloadInputs { get; set; }
    }

    public class ArtifactSourceDefinitionUrl
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class ArtifactSourceVersionUrl
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Branch
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Branches
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class BuildUri
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class Collection
    {
        public string id { get; set; }
        public string baseUrl { get; set; }
    }

    public class Condition
    {
        public string name { get; set; }
        public string conditionType { get; set; }
        public string value { get; set; }
        public bool result { get; set; }
    }

    public class Data
    {
        public string releaseProperties { get; set; }
        public string environmentStatuses { get; set; }
        public List<object> workItems { get; set; }
        public PreviousReleaseEnvironment previousReleaseEnvironment { get; set; }
        public List<object> commits { get; set; }
        public TestResults testResults { get; set; }
        public string moreWorkItemsMessage { get; set; }
    }

    public class DefaultVersionType
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Definition
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class DefinitionReference
    {
        public ArtifactSourceDefinitionUrl artifactSourceDefinitionUrl { get; set; }
        public Branches branches { get; set; }
        public BuildUri buildUri { get; set; }
        public Definition definition { get; set; }
        public IsMultiDefinitionType IsMultiDefinitionType { get; set; }
        public IsXamlBuildArtifactType IsXamlBuildArtifactType { get; set; }
        public Project project { get; set; }

        [JsonProperty("repository.provider")]
        public RepositoryProvider repositoryprovider { get; set; }
        public Repository repository { get; set; }
        public RequestedFor requestedFor { get; set; }
        public RequestedForId requestedForId { get; set; }
        public SourceVersion sourceVersion { get; set; }
        public Version version { get; set; }
        public ArtifactSourceVersionUrl artifactSourceVersionUrl { get; set; }
        public DefaultVersionType defaultVersionType { get; set; }
        public Branch branch { get; set; }
    }

    public class Deployment
    {
        public int id { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseEnvironment releaseEnvironment { get; set; }
        public object projectReference { get; set; }
        public int definitionEnvironmentId { get; set; }
        public int attempt { get; set; }
        public string reason { get; set; }
        public string deploymentStatus { get; set; }
        public string operationStatus { get; set; }
        public RequestedBy requestedBy { get; set; }
        public RequestedFor requestedFor { get; set; }
        public DateTime queuedOn { get; set; }
        public DateTime startedOn { get; set; }
        public DateTime completedOn { get; set; }
        public DateTime lastModifiedOn { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public List<Condition> conditions { get; set; }
        public List<PreDeployApproval> preDeployApprovals { get; set; }
        public List<PostDeployApproval> postDeployApprovals { get; set; }
        public Links _links { get; set; }
    }

    public class DeploymentInput
    {
        public ParallelExecution parallelExecution { get; set; }
        public object agentSpecification { get; set; }
        public bool skipArtifactsDownload { get; set; }
        public ArtifactsDownloadInput artifactsDownloadInput { get; set; }
        public int queueId { get; set; }
        public List<object> demands { get; set; }
        public bool enableAccessToken { get; set; }
        public int timeoutInMinutes { get; set; }
        public int jobCancelTimeoutInMinutes { get; set; }
        public string condition { get; set; }
        public OverrideInputs overrideInputs { get; set; }
    }

    public class DeploymentJob
    {
        public Job job { get; set; }
        public List<Task> tasks { get; set; }
    }

    public class DeployPhasesSnapshot
    {
        public DeploymentInput deploymentInput { get; set; }
        public int rank { get; set; }
        public string phaseType { get; set; }
        public string name { get; set; }
        public object refName { get; set; }
        public List<object> workflowTasks { get; set; }
    }

    public class DeployStep
    {
        public int id { get; set; }
        public int deploymentId { get; set; }
        public int attempt { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
        public string operationStatus { get; set; }
        public List<ReleaseDeployPhase> releaseDeployPhases { get; set; }
        public RequestedBy requestedBy { get; set; }
        public RequestedFor requestedFor { get; set; }
        public DateTime queuedOn { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public DateTime lastModifiedOn { get; set; }
        public bool hasStarted { get; set; }
        public List<object> tasks { get; set; }
        public string runPlanId { get; set; }
        public List<object> issues { get; set; }
    }

    public class DetailedMessage
    {
        public string text { get; set; }
        public string html { get; set; }
        public string markdown { get; set; }
    }

    public class Environment
    {
        public int id { get; set; }
        public int releaseId { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public Variables variables { get; set; }
        public List<object> variableGroups { get; set; }
        public List<PreDeployApproval> preDeployApprovals { get; set; }
        public List<PostDeployApproval> postDeployApprovals { get; set; }
        public PreApprovalsSnapshot preApprovalsSnapshot { get; set; }
        public PostApprovalsSnapshot postApprovalsSnapshot { get; set; }
        public List<DeployStep> deploySteps { get; set; }
        public int rank { get; set; }
        public int definitionEnvironmentId { get; set; }
        public EnvironmentOptions environmentOptions { get; set; }
        public List<object> demands { get; set; }
        public List<Condition> conditions { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public List<object> workflowTasks { get; set; }
        public List<DeployPhasesSnapshot> deployPhasesSnapshot { get; set; }
        public Owner owner { get; set; }
        public List<object> schedules { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseCreatedBy releaseCreatedBy { get; set; }
        public string triggerReason { get; set; }
        public double timeToDeploy { get; set; }
        public ProcessParameters processParameters { get; set; }
        public PreDeploymentGatesSnapshot preDeploymentGatesSnapshot { get; set; }
        public PostDeploymentGatesSnapshot postDeploymentGatesSnapshot { get; set; }
    }

    public class EnvironmentOptions
    {
        public string emailNotificationType { get; set; }
        public string emailRecipients { get; set; }
        public bool skipArtifactsDownload { get; set; }
        public int timeoutInMinutes { get; set; }
        public bool enableAccessToken { get; set; }
        public bool publishDeploymentStatus { get; set; }
        public bool badgeEnabled { get; set; }
        public bool autoLinkWorkItems { get; set; }
        public bool pullRequestDeploymentEnabled { get; set; }
    }

    public class IsMultiDefinitionType
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class IsXamlBuildArtifactType
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class Job
    {
        public int id { get; set; }
        public string timelineRecordId { get; set; }
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string status { get; set; }
        public int rank { get; set; }
        public List<object> issues { get; set; }
        public string agentName { get; set; }
    }

    public class LastModifiedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Links
    {
        public Avatar avatar { get; set; }
        public Web web { get; set; }
        public Self self { get; set; }
        public Logs logs { get; set; }
    }

    public class Logs
    {
        public string href { get; set; }
    }

    public class Message
    {
        public string text { get; set; }
        public string html { get; set; }
        public string markdown { get; set; }
    }

    public class OverrideInputs
    {
    }

    public class Owner
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class ParallelExecution
    {
        public string parallelExecutionType { get; set; }
    }

    public class PostApprovalsSnapshot
    {
        public List<object> approvals { get; set; }
    }

    public class PostDeployApproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseEnvironment releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class PostDeploymentGatesSnapshot
    {
        public int id { get; set; }
        public object gatesOptions { get; set; }
        public List<object> gates { get; set; }
    }

    public class PreApprovalsSnapshot
    {
        public List<object> approvals { get; set; }
    }

    public class PreDeployApproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        public Release release { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public ReleaseEnvironment releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class PreDeploymentGatesSnapshot
    {
        public int id { get; set; }
        public object gatesOptions { get; set; }
        public List<object> gates { get; set; }
    }

    public class PreviousReleaseEnvironment
    {
        public string status { get; set; }
    }

    public class ProcessParameters
    {
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string baseUrl { get; set; }
    }

    public class ProjectReference
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public List<Artifact> artifacts { get; set; }
        public string webAccessUri { get; set; }
        public string description { get; set; }
        public string reason { get; set; }
    }

    public class ReleaseCreatedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class ReleaseDefinition
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
    }

    public class ReleaseDeployPhase
    {
        public int id { get; set; }
        public string phaseId { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public string phaseType { get; set; }
        public string status { get; set; }
        public string runPlanId { get; set; }
        public List<DeploymentJob> deploymentJobs { get; set; }
        public List<object> manualInterventions { get; set; }
        public DateTime startedOn { get; set; }
    }

    public class ReleaseEnvironment
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
    }

    public class Repository
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RepositoryProvider
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class RequestedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class RequestedFor
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
        public object name { get; set; }
    }

    public class RequestedForId
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class Resource
    {
        public Environment environment { get; set; }
        public Project project { get; set; }
        public Deployment deployment { get; set; }
        public object comment { get; set; }
        public Data data { get; set; }
        public string stageName { get; set; }
        public int attemptId { get; set; }
        public int id { get; set; }
        public string url { get; set; }
    }

    public class ResourceContainers
    {
        public Collection collection { get; set; }
        public Server server { get; set; }
        public Project project { get; set; }
    }

    public class Root
    {
        public string subscriptionId { get; set; }
        public int notificationId { get; set; }
        public string id { get; set; }
        public string eventType { get; set; }
        public string publisherId { get; set; }
        public Message message { get; set; }
        public DetailedMessage detailedMessage { get; set; }
        public Resource resource { get; set; }
        public string resourceVersion { get; set; }
        public ResourceContainers resourceContainers { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Server
    {
        public string id { get; set; }
        public string baseUrl { get; set; }
    }

    public class SourceVersion
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class Task
    {
        public string id { get; set; }
        public string timelineRecordId { get; set; }
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string status { get; set; }
        public int rank { get; set; }
        public List<object> issues { get; set; }
        public string agentName { get; set; }
        public Task task { get; set; }
    }

    public class Task2
    {
        public string id { get; set; }
        public string name { get; set; }
        public string version { get; set; }
    }

    public class TestResults
    {
    }

    public class Variables
    {
    }

    public class Version
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }


}
