﻿using Microsoft.DataTransfer.Basics;
using Microsoft.DataTransfer.DocumentDb.Wpf.Shared;
using Microsoft.DataTransfer.WpfHost.Extensibility;
using System;
using System.Collections.Generic;

namespace Microsoft.DataTransfer.DocumentDb.Wpf.Sink
{
    abstract class DocumentDbSinkAdapterConfigurationProvider<TConfiguration> : DocumentDbAdapterConfigurationProvider<TConfiguration, ISharedDocumentDbSinkAdapterConfiguration>
        where TConfiguration : DocumentDbSinkAdapterConfiguration
    {
        private readonly IImportSharedStorage sharedStorage;

        public DocumentDbSinkAdapterConfigurationProvider(IImportSharedStorage sharedStorage)
        {
            Guard.NotNull("sharedStorage", sharedStorage);
            this.sharedStorage = sharedStorage;
        }

        protected override void PopulateCommandLineArguments(TConfiguration configuration, IDictionary<string, string> arguments)
        {
            base.PopulateCommandLineArguments(configuration, arguments);

            Guard.NotNull("configuration", configuration);
            Guard.NotNull("arguments", arguments);

            arguments.Add(DocumentDbSinkAdapterConfiguration.CollectionPropertyName, AsCollectionArgument(configuration.Collection));

            if (!String.IsNullOrEmpty(configuration.PartitionKey))
                arguments.Add(DocumentDbSinkAdapterConfiguration.PartitionKeyPropertyName, configuration.PartitionKey);

            if (configuration.CollectionTier.HasValue && configuration.CollectionTier.Value != Defaults.Current.SinkCollectionTier)
                arguments.Add(DocumentDbSinkAdapterConfiguration.CollectionTierPropertyName, configuration.CollectionTier.Value.ToString());

            if (configuration.UseIndexingPolicyFile)
            {
                if (!String.IsNullOrEmpty(configuration.IndexingPolicyFile))
                    arguments.Add(DocumentDbSinkAdapterConfiguration.IndexingPolicyFilePropertyName, configuration.IndexingPolicyFile);
            }
            else
            {
                if (!String.IsNullOrEmpty(configuration.IndexingPolicy))
                    arguments.Add(DocumentDbSinkAdapterConfiguration.IndexingPolicyPropertyName, configuration.IndexingPolicy);
            }

            if (!String.IsNullOrEmpty(configuration.IdField))
                arguments.Add(DocumentDbSinkAdapterConfiguration.IdFieldPropertyName, configuration.IdField);

            if (configuration.DisableIdGeneration)
                arguments.Add(DocumentDbSinkAdapterConfiguration.DisableIdGenerationPropertyName, null);

            if (configuration.UpdateExisting)
                arguments.Add(DocumentDbSinkAdapterConfiguration.UpdateExistingPropertyName, null);

            if (configuration.Dates.HasValue && configuration.Dates.Value != Defaults.Current.SinkDateTimeHandling)
                arguments.Add(DocumentDbSinkAdapterConfiguration.DatesPropertyName, configuration.Dates.Value.ToString());
        }

        protected ISharedDocumentDbSinkAdapterConfiguration GetSharedConfiguration()
        {
            return sharedStorage.GetOrAdd<ISharedDocumentDbSinkAdapterConfiguration>(
                SharedDocumentDbSinkAdapterConfigurationKey.Value, CreateNewSharedConfiguration);
        }

        private static ISharedDocumentDbSinkAdapterConfiguration CreateNewSharedConfiguration(object key)
        {
            return new SharedDocumentDbSinkAdapterConfiguration();
        }
    }
}
