using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace NotificationsBot.Models.AzureModels.Release
{
    public class Artifact
    {
        [AllowNull]
        public string sourceId { get; set; }
        [AllowNull]
        public string type { get; set; }
        [AllowNull]
        public string alias { get; set; }
        [AllowNull]
        public DefinitionReference definitionReference { get; set; }
        public bool isPrimary { get; set; }
        public bool isRetained { get; set; }
    }

    public class ArtifactsDownloadInput
    {
        [AllowNull]
        public List<object> downloadInputs { get; set; }
    }

    public class ArtifactSourceDefinitionUrl
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class ArtifactSourceVersionUrl
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class Avatar
    {
        [AllowNull]
        public string href { get; set; }
    }

    public class Branch
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class Branches
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class BuildUri
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class Collection
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string baseUrl { get; set; }
    }

    public class Condition
    {
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public string conditionType { get; set; }
        [AllowNull]
        public string value { get; set; }
        public bool result { get; set; }
    }

    public class Data
    {
        [AllowNull]
        public string releaseProperties { get; set; }
        [AllowNull]
        public string environmentStatuses { get; set; }
        [AllowNull]
        public List<object> workItems { get; set; }
        [AllowNull]
        public PreviousReleaseEnvironment previousReleaseEnvironment { get; set; }
        [AllowNull]
        public List<object> commits { get; set; }
        [AllowNull]
        public TestResults testResults { get; set; }
        [AllowNull]
        public string moreWorkItemsMessage { get; set; }
    }

    public class DefaultVersionType
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class Definition
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class DefinitionReference
    {
        [AllowNull]
        public ArtifactSourceDefinitionUrl artifactSourceDefinitionUrl { get; set; }
        [AllowNull]
        public Branches branches { get; set; }
        [AllowNull]
        public BuildUri buildUri { get; set; }
        [AllowNull]
        public Definition definition { get; set; }
        [AllowNull]
        public IsMultiDefinitionType IsMultiDefinitionType { get; set; }
        [AllowNull]
        public IsXamlBuildArtifactType IsXamlBuildArtifactType { get; set; }
        [AllowNull]
        public Project project { get; set; }
        [JsonProperty("repository.provider")]
        [AllowNull]
        public RepositoryProvider repositoryprovider { get; set; }
        [AllowNull]
        public Repository repository { get; set; }
        [AllowNull]
        public RequestedFor requestedFor { get; set; }
        [AllowNull]
        public RequestedForId requestedForId { get; set; }
        [AllowNull]
        public SourceVersion sourceVersion { get; set; }
        [AllowNull]
        public Version version { get; set; }
        [AllowNull]
        public ArtifactSourceVersionUrl artifactSourceVersionUrl { get; set; }
        [AllowNull]
        public DefaultVersionType defaultVersionType { get; set; }
        [AllowNull]
        public Branch branch { get; set; }
    }

    public class Deployment
    {
        public int id { get; set; }
        [AllowNull]
        public Release release { get; set; }
        [AllowNull]
        public ReleaseDefinition releaseDefinition { get; set; }
        [AllowNull]
        public ReleaseEnvironment releaseEnvironment { get; set; }
        [AllowNull]
        public object projectReference { get; set; }
        public int definitionEnvironmentId { get; set; }
        public int attempt { get; set; }
        [AllowNull]
        public string reason { get; set; }
        [AllowNull]
        public string deploymentStatus { get; set; }
        [AllowNull]
        public string operationStatus { get; set; }
        [AllowNull]
        public RequestedBy requestedBy { get; set; }
        [AllowNull]
        public RequestedFor requestedFor { get; set; }
        public DateTime queuedOn { get; set; }
        public DateTime startedOn { get; set; }
        public DateTime completedOn { get; set; }
        public DateTime lastModifiedOn { get; set; }
        [AllowNull]
        public LastModifiedBy lastModifiedBy { get; set; }
        [AllowNull]
        public List<Condition> conditions { get; set; }
        [AllowNull]
        public List<PreDeployApproval> preDeployApprovals { get; set; }
        [AllowNull]
        public List<PostDeployApproval> postDeployApprovals { get; set; }
        [AllowNull]
        public Links _links { get; set; }
    }

    public class DeploymentInput
    {
        [AllowNull]
        public ParallelExecution parallelExecution { get; set; }
        [AllowNull]
        public object agentSpecification { get; set; }
        public bool skipArtifactsDownload { get; set; }
        [AllowNull]
        public ArtifactsDownloadInput artifactsDownloadInput { get; set; }
        public int queueId { get; set; }
        [AllowNull]
        public List<object> demands { get; set; }
        public bool enableAccessToken { get; set; }
        public int timeoutInMinutes { get; set; }
        public int jobCancelTimeoutInMinutes { get; set; }
        [AllowNull]
        public string condition { get; set; }
        [AllowNull]
        public OverrideInputs overrideInputs { get; set; }
    }

    public class DeploymentJob
    {
        [AllowNull]
        public Job job { get; set; }
        [AllowNull]
        public List<Task> tasks { get; set; }
    }

    public class DeployPhasesSnapshot
    {
        [AllowNull]
        public DeploymentInput deploymentInput { get; set; }
        public int rank { get; set; }
        [AllowNull]
        public string phaseType { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public object refName { get; set; }
        [AllowNull]
        public List<object> workflowTasks { get; set; }
    }

    

    public class Environment
    {
        public int id { get; set; }
        public int releaseId { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public string status { get; set; }
        [AllowNull]
        public Variables variables { get; set; }
        [AllowNull]
        public List<object> variableGroups { get; set; }
        [AllowNull]
        public List<PreDeployApproval> preDeployApprovals { get; set; }
        [AllowNull]
        public List<PostDeployApproval> postDeployApprovals { get; set; }
        [AllowNull]
        public PreApprovalsSnapshot preApprovalsSnapshot { get; set; }
        [AllowNull]
        public PostApprovalsSnapshot postApprovalsSnapshot { get; set; }
        [AllowNull]
        public List<DeployStep> deploySteps { get; set; }
        public int rank { get; set; }
        public int definitionEnvironmentId { get; set; }
        [AllowNull]
        public EnvironmentOptions environmentOptions { get; set; }
        [AllowNull]
        public List<object> demands { get; set; }
        [AllowNull]
        public List<Condition> conditions { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        [AllowNull]
        public List<object> workflowTasks { get; set; }
        [AllowNull]
        public List<DeployPhasesSnapshot> deployPhasesSnapshot { get; set; }
        [AllowNull]
        public Owner owner { get; set; }
        [AllowNull]
        public List<object> schedules { get; set; }
        [AllowNull]
        public Release release { get; set; }
        [AllowNull]
        public ReleaseDefinition releaseDefinition { get; set; }
        [AllowNull]
        public ReleaseCreatedBy releaseCreatedBy { get; set; }
        [AllowNull]
        public string triggerReason { get; set; }
        public double timeToDeploy { get; set; }
        [AllowNull]
        public ProcessParameters processParameters { get; set; }
        [AllowNull]
        public PreDeploymentGatesSnapshot preDeploymentGatesSnapshot { get; set; }
        [AllowNull]
        public PostDeploymentGatesSnapshot postDeploymentGatesSnapshot { get; set; }
    }

    public class EnvironmentOptions
    {
        [AllowNull]
        public string emailNotificationType { get; set; }
        [AllowNull]
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
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class IsXamlBuildArtifactType
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class Job
    {
        public int id { get; set; }
        [AllowNull]
        public string timelineRecordId { get; set; }
        [AllowNull]
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        [AllowNull]
        public string status { get; set; }
        public int rank { get; set; }
        [AllowNull]
        public List<object> issues { get; set; }
        [AllowNull]
        public string agentName { get; set; }
    }

    public class LastModifiedBy
    {
        [AllowNull]
        public string displayName { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string uniqueName { get; set; }
        [AllowNull]
        public string imageUrl { get; set; }
        [AllowNull]
        public string descriptor { get; set; }
    }

    public class Links
    {
        [AllowNull]
        public Avatar avatar { get; set; }
        [AllowNull]
        public Web web { get; set; }
        [AllowNull]
        public Self self { get; set; }
        [AllowNull]
        public Logs logs { get; set; }
    }

    public class Logs
    {
        [AllowNull]
        public string href { get; set; }
    }

    public class Message
    {
        [AllowNull]
        public string text { get; set; }
        [AllowNull]
        public string html { get; set; }
        [AllowNull]
        public string markdown { get; set; }
    }

    public class OverrideInputs
    {
    }

    public class Owner
    {
        [AllowNull]
        public string displayName { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string uniqueName { get; set; }
        [AllowNull]
        public string imageUrl { get; set; }
        [AllowNull]
        public string descriptor { get; set; }
    }

    public class ParallelExecution
    {
        [AllowNull]
        public string parallelExecutionType { get; set; }
    }

    public class PostApprovalsSnapshot
    {
        [AllowNull]
        public List<object> approvals { get; set; }
    }

    public class PostDeployApproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        [AllowNull]
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        [AllowNull]
        public string status { get; set; }
        [AllowNull]
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        [AllowNull]
        public Release release { get; set; }
        [AllowNull]
        public ReleaseDefinition releaseDefinition { get; set; }
        [AllowNull]
        public ReleaseEnvironment releaseEnvironment { get; set; }
        [AllowNull]
        public string url { get; set; }
    }

    public class PostDeploymentGatesSnapshot
    {
        public int id { get; set; }
        [AllowNull]
        public object gatesOptions { get; set; }
        [AllowNull]
        public List<object> gates { get; set; }
    }

    public class PreApprovalsSnapshot
    {
        [AllowNull]
        public List<object> approvals { get; set; }
    }

    public class PreDeployApproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        [AllowNull]
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        [AllowNull]
        public string status { get; set; }
        [AllowNull]
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        [AllowNull]
        public Release release { get; set; }
        [AllowNull]
        public ReleaseDefinition releaseDefinition { get; set; }
        [AllowNull]
        public ReleaseEnvironment releaseEnvironment { get; set; }
        [AllowNull]
        public string url { get; set; }
    }

    public class PreDeploymentGatesSnapshot
    {
        public int id { get; set; }
        [AllowNull]
        public object gatesOptions { get; set; }
        [AllowNull]
        public List<object> gates { get; set; }
    }

    public class PreviousReleaseEnvironment
    {
        [AllowNull]
        public string status { get; set; }
    }

    public class ProcessParameters
    {
    }

    public class Project
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public string baseUrl { get; set; }
    }

    public class ProjectReference
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
        [AllowNull]
        public List<Artifact> artifacts { get; set; }
        [AllowNull]
        public string webAccessUri { get; set; }
        [AllowNull]
        public string description { get; set; }
        [AllowNull]
        public string reason { get; set; }
    }

    public class ReleaseCreatedBy
    {
        [AllowNull]
        public string displayName { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string uniqueName { get; set; }
        [AllowNull]
        public string imageUrl { get; set; }
        [AllowNull]
        public string descriptor { get; set; }
    }

    public class ReleaseDefinition
    {
        public int id { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public string path { get; set; }
        [AllowNull]
        public object projectReference { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
    }

    public class ReleaseDeployPhase
    {
        public int id { get; set; }
        [AllowNull]
        public string phaseId { get; set; }
        [AllowNull]
        public string name { get; set; }
        public int rank { get; set; }
        [AllowNull]
        public string phaseType { get; set; }
        [AllowNull]
        public string status { get; set; }
        [AllowNull]
        public string runPlanId { get; set; }
        [AllowNull]
        public List<DeploymentJob> deploymentJobs { get; set; }
        [AllowNull]
        public List<object> manualInterventions { get; set; }
        public DateTime startedOn { get; set; }
    }

    public class ReleaseEnvironment
    {
        public int id { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
    }

    public class Repository
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class RepositoryProvider
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class RequestedBy
    {
        [AllowNull]
        public string displayName { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string uniqueName { get; set; }
        [AllowNull]
        public string imageUrl { get; set; }
        [AllowNull]
        public string descriptor { get; set; }
    }

    public class RequestedFor
    {
        [AllowNull]
        public string displayName { get; set; }
        [AllowNull]
        public string url { get; set; }
        [AllowNull]
        public Links _links { get; set; }
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string uniqueName { get; set; }
        [AllowNull]
        public string imageUrl { get; set; }
        [AllowNull]
        public string descriptor { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class RequestedForId
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class Resource
    {
        [AllowNull]
        public Environment environment { get; set; }
        [AllowNull]
        public Project project { get; set; }
        [AllowNull]
        public Deployment deployment { get; set; }
        [AllowNull]
        public object comment { get; set; }
        [AllowNull]
        public Data data { get; set; }
        [AllowNull]
        public string stageName { get; set; }
        public int attemptId { get; set; }
        public int id { get; set; }
        [AllowNull]
        public string url { get; set; }
    }

    public class ResourceContainers
    {
        [AllowNull]
        public Collection collection { get; set; }
        [AllowNull]
        public Server server { get; set; }
        [AllowNull]
        public Project project { get; set; }
    }

    public class Root
    {
        [AllowNull]
        public string subscriptionId { get; set; }
        public int notificationId { get; set; }
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string eventType { get; set; }
        [AllowNull]
        public string publisherId { get; set; }
        [AllowNull]
        public Message message { get; set; }
        [AllowNull]
        public DetailedMessage detailedMessage { get; set; }
        [AllowNull]
        public Resource resource { get; set; }
        [AllowNull]
        public string resourceVersion { get; set; }
        [AllowNull]
        public ResourceContainers resourceContainers { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class DetailedMessage
    {
        [AllowNull]
        public string text { get; set; }
        [AllowNull]
        public string html { get; set; }
        [AllowNull]
        public string markdown { get; set; }
    }

    public class DeployStep
    {
        public int id { get; set; }
        public int deploymentId { get; set; }
        public int attempt { get; set; }
        [AllowNull]
        public string reason { get; set; }
        [AllowNull]
        public string status { get; set; }
        [AllowNull]
        public string operationStatus { get; set; }
        [AllowNull]
        public List<ReleaseDeployPhase> releaseDeployPhases { get; set; }
        [AllowNull]
        public RequestedBy requestedBy { get; set; }
        [AllowNull]
        public RequestedFor requestedFor { get; set; }
        public DateTime queuedOn { get; set; }
        [AllowNull]
        public LastModifiedBy lastModifiedBy { get; set; }
        public DateTime lastModifiedOn { get; set; }
        public bool hasStarted { get; set; }
        [AllowNull]
        public List<object> tasks { get; set; }
        [AllowNull]
        public string runPlanId { get; set; }
        [AllowNull]
        public List<object> issues { get; set; }
    }

    public class Self
    {
        [AllowNull]
        public string href { get; set; }
    }

    public class Server
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string baseUrl { get; set; }
    }

    public class SourceVersion
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public object name { get; set; }
    }

    public class Task
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string timelineRecordId { get; set; }
        [AllowNull]
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        [AllowNull]
        public string status { get; set; }
        public int rank { get; set; }
        [AllowNull]
        public List<Issue> issues { get; set; }
        [AllowNull]
        public string agentName { get; set; }
        [AllowNull]
        public Task task { get; set; }
    }

    public class Task2
    {
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
        [AllowNull]
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
        [AllowNull]
        public string id { get; set; }
        [AllowNull]
        public string name { get; set; }
    }

    public class Web
    {
        [AllowNull]
        public string href { get; set; }
    }

    public class Issue
    {
        [AllowNull]
        public string issueType { get; set; }
        [AllowNull]
        public string message { get; set; }
        [AllowNull]
        public Data data { get; set; }
    }
}