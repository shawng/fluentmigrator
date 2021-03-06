﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FluentMigrator.Infrastructure;
using System.Linq;
using FluentMigrator.VersionTableInfo;

namespace FluentMigrator.Runner
{
	public class MigrationLoader : IMigrationLoader
	{
		public IMigrationConventions Conventions { get; private set; }

		public MigrationLoader(IMigrationConventions conventions)
		{
			Conventions = conventions;
		}

		public IEnumerable<MigrationMetadata> FindMigrationsIn(Assembly assembly, string @namespace)
		{
			IEnumerable<Type> matchedTypes = assembly.GetExportedTypes().Where(t => Conventions.TypeIsMigration(t));

			if (!string.IsNullOrEmpty(@namespace))
				matchedTypes = assembly.GetExportedTypes().Where(t => t.Namespace == @namespace && Conventions.TypeIsMigration(t));

			foreach (Type type in matchedTypes)
				yield return Conventions.GetMetadataForMigration(type);
		}

		public IVersionTableMetaData GetVersionTableMetaData(Assembly assembly)
		{
			Type matchedType = assembly.GetExportedTypes().Where(t => Conventions.TypeIsVersionTableMetaData(t)).FirstOrDefault();

			if (matchedType == null)
			{
				return new DefaultVersionTableMetaData();
			}

			return (IVersionTableMetaData)Activator.CreateInstance(matchedType);
		}
	}
}