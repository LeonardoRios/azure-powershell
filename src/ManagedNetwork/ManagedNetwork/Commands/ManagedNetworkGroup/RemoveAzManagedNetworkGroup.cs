﻿using System.Management.Automation;
using Microsoft.Azure.Commands.ManagedNetwork.Common;
using Microsoft.Azure.Management.ManagedNetwork;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Commands.ManagedNetwork.Models;
using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;

namespace Microsoft.Azure.Commands.ManagedNetwork
{
    /// <summary>
    /// New Azure InputObject Command-let
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzManagedNetworkGroup", SupportsShouldProcess = true, DefaultParameterSetName = Constants.NameParameterSet)]
    [OutputType(typeof(PSManagedNetwork))]
    public class RemoveAzManagedNetworkGroup : AzureManagedNetworkCmdletBase
    {
        /// <summary>
        /// Gets or sets The Resource Group name
        /// </summary>
        [Parameter(Position = 0, 
            Mandatory = true, 
            HelpMessage = Constants.ResourceGroupNameHelp, 
            ParameterSetName = Constants.NameParameterSet)]
        [ValidateNotNullOrEmpty]
        [ResourceGroupCompleter]
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the Managed Network Name
        /// </summary>
        [Parameter(Position = 1, 
            Mandatory = true, 
            HelpMessage = Constants.ManagedNetworkNameHelp,
            ParameterSetName = Constants.NameParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ManagedNetworkName { get; set; }

        /// <summary>
        /// Gets or sets the Managed Network Name
        /// </summary>
        [Parameter(Position = 1,
            Mandatory = true,
            HelpMessage = Constants.ManagedNetworkGroupNameHelp,
            ParameterSetName = Constants.NameParameterSet)]
        [Parameter(Mandatory = true,
            HelpMessage = Constants.ManagedNetworkGroupNameHelp,
            ParameterSetName = Constants.ManagedNetworkObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        [ResourceNameCompleter("Microsoft.ManagedNetwork/managedNetworks/managednetworkgroups", "ResourceGroupName", "ManagedNetworkName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ARM resource ID
        /// </summary>
        [Parameter(Mandatory = true, 
            HelpMessage = Constants.ResourceIdNameHelp,
            ParameterSetName = Constants.ResourceIdParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the mangaged Network Group
        /// </summary>
        [Parameter(Mandatory = true,
            HelpMessage = Constants.InputObjectHelp,
            ParameterSetName = Constants.InputObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        public PSManagedNetworkGroup InputObject { get; set; }

        /// <summary>
        /// Gets or sets the managed network
        /// </summary>
        [Parameter(Mandatory = true,
            HelpMessage = Constants.ManagedNetworkObjectHelp,
            ParameterSetName = Constants.ManagedNetworkObjectParameterSet)]
        [ValidateNotNullOrEmpty]
        public PSManagedNetwork ManagedNetworkObject { get; set; }

        [Parameter(Mandatory = false, HelpMessage = Constants.PassThruHelp)]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        ///     The AsJob parameter to run in the background.
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = Constants.ForceHelp)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        ///     The AsJob parameter to run in the background.
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = Constants.AsJobHelp)]
        public SwitchParameter AsJob { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            if (string.Equals(
                this.ParameterSetName,
                Constants.ResourceIdParameterSet))
            {
                var resourceIdentifier = new ResourceIdentifier(this.ResourceId);
                this.ResourceGroupName = resourceIdentifier.ResourceGroupName;
                this.ManagedNetworkName = resourceIdentifier.ParentResource.Split('/')[1];
                this.Name = resourceIdentifier.ResourceName;
            }
            else if (string.Equals(
                    this.ParameterSetName,
                    Constants.InputObjectParameterSet))
            {
                var resourceIdentifier = new ResourceIdentifier(this.InputObject.Id);
                this.ResourceGroupName = resourceIdentifier.ResourceGroupName;
                this.ManagedNetworkName = resourceIdentifier.ParentResource.Split('/')[1];
                this.Name = resourceIdentifier.ResourceName;
            }
            else if (string.Equals(
                    this.ParameterSetName,
                    Constants.ManagedNetworkObjectParameterSet))
            {
                var resourceIdentifier = new ResourceIdentifier(this.ManagedNetworkObject.Id);
                this.ResourceGroupName = resourceIdentifier.ResourceGroupName;
                this.ManagedNetworkName = resourceIdentifier.ResourceName;
            }


            var present = IsManagedNetworkPresent(ResourceGroupName, ManagedNetworkName);
            ConfirmAction(
                Force.IsPresent,
                string.Format(Constants.ConfirmDeleteResource, ManagedNetworkName),
                Constants.DeletingResource,
                Name,
                () =>
                {
                    RemoveManagedNetworkGroup();
                    if (this.PassThru.IsPresent)
                    {
                        WriteObject(true);
                    }
                },
                () => present);
        }
        private void RemoveManagedNetworkGroup()
        {
            this.ManagedNetworkManagementClient.ManagedNetworkGroups.Delete(this.ResourceGroupName, this.ManagedNetworkName, this.Name);
        }
    }
}
