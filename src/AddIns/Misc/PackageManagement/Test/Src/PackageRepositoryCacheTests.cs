﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO.Packaging;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageRepositoryCacheTests
	{
		PackageRepositoryCache cache;
		FakePackageRepositoryFactory fakePackageRepositoryFactory;
		PackageSource nuGetPackageSource;
		OneRegisteredPackageSourceHelper packageSourcesHelper;
		RecentPackageInfo[] recentPackagesPassedToCreateRecentPackageRepository;
		FakePackageRepository fakeAggregateRepositoryPassedToCreateRecentPackageRepository;

		void CreateCache()
		{
			CreatePackageSources();
			CreateCacheUsingPackageSources();
		}
		
		void CreatePackageSources()
		{
			packageSourcesHelper = new OneRegisteredPackageSourceHelper();
		}
		
		void CreateCacheUsingPackageSources()
		{
			nuGetPackageSource = new PackageSource("http://nuget.org", "NuGet");
			fakePackageRepositoryFactory = new FakePackageRepositoryFactory();
			RegisteredPackageSources packageSources = packageSourcesHelper.Options.PackageSources;
			IList<RecentPackageInfo> recentPackages = packageSourcesHelper.Options.RecentPackages;
			cache = new PackageRepositoryCache(fakePackageRepositoryFactory, packageSources, recentPackages);
		}
		
		FakePackageRepository AddFakePackageRepositoryForPackageSource(string source)
		{
			return fakePackageRepositoryFactory.AddFakePackageRepositoryForPackageSource(source);
		}
		
		IPackageRepository CreateRecentPackageRepositoryPassingAggregateRepository()
		{
			recentPackagesPassedToCreateRecentPackageRepository = new RecentPackageInfo[0];
			fakeAggregateRepositoryPassedToCreateRecentPackageRepository = new FakePackageRepository();
			
			return cache.CreateRecentPackageRepository(
				recentPackagesPassedToCreateRecentPackageRepository,
				fakeAggregateRepositoryPassedToCreateRecentPackageRepository);
		}
		
		RecentPackageInfo AddOneRecentPackage()
		{
			var recentPackage = new RecentPackageInfo("Id", new SemanticVersion("1.0"));
			packageSourcesHelper.Options.RecentPackages.Add(recentPackage);
			return recentPackage;
		}

		[Test]
		public void CreateRepository_CacheCastToISharpDevelopPackageRepositoryFactory_CreatesPackageRepositoryUsingPackageRepositoryFactoryPassedInConstructor()
		{
			CreateCache();
			var factory = cache as ISharpDevelopPackageRepositoryFactory;
			IPackageRepository repository = factory.CreateRepository(nuGetPackageSource.Source);
			
			Assert.AreEqual(fakePackageRepositoryFactory.FakePackageRepository, repository);
		}
		
		[Test]
		public void CreateRepository_PackageSourcePassed_PackageSourceUsedToCreateRepository()
		{
			CreateCache();
			cache.CreateRepository(nuGetPackageSource.Source);
			
			string actualPackageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			Assert.AreEqual(nuGetPackageSource.Source, actualPackageSource);
		}
		
		[Test]
		public void CreateRepository_RepositoryAlreadyCreatedForPackageSource_NoRepositoryCreated()
		{
			CreateCache();
			cache.CreateRepository(nuGetPackageSource.Source);
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			cache.CreateRepository(nuGetPackageSource.Source);
			
			Assert.AreEqual(0, fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Count);
		}
		
		[Test]
		public void CreateRepository_RepositoryAlreadyCreatedForPackageSource_RepositoryOriginallyCreatedIsReturned()
		{
			CreateCache();
			IPackageRepository originallyCreatedRepository = cache.CreateRepository(nuGetPackageSource.Source);
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			IPackageRepository repository = cache.CreateRepository(nuGetPackageSource.Source);
			
			Assert.AreSame(originallyCreatedRepository, repository);
		}
		
		[Test]
		public void CreateSharedRepository_MethodCalled_ReturnsSharedPackageRepository()
		{
			CreateCache();
			ISharedPackageRepository repository = cache.CreateSharedRepository(null, null, null);
			Assert.IsNotNull(repository);
		}
		
		[Test]
		public void CreatedSharedRepository_PathResolverPassed_PathResolverUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakePackagePathResolver resolver = new FakePackagePathResolver();
			cache.CreateSharedRepository(resolver, null, null);
			
			Assert.AreEqual(resolver, fakePackageRepositoryFactory.PathResolverPassedToCreateSharedRepository);
		}
		
		[Test]
		public void CreatedSharedRepository_FileSystemPassed_FileSystemUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakeFileSystem fileSystem = new FakeFileSystem();
			cache.CreateSharedRepository(null, fileSystem, null);
			
			Assert.AreEqual(fileSystem, fakePackageRepositoryFactory.FileSystemPassedToCreateSharedRepository);
		}
		
		[Test]
		public void CreatedSharedRepository_ConfigSettingsFileSystemPassed_FileSystemUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakeFileSystem fileSystem = new FakeFileSystem();
			cache.CreateSharedRepository(null, null, fileSystem);
			
			Assert.AreEqual(fileSystem, fakePackageRepositoryFactory.ConfigSettingsFileSystemPassedToCreateSharedRepository);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageRepositories_ReturnsAggregateRepositoryFromFactory()
		{
			CreatePackageSources();
			packageSourcesHelper.AddTwoPackageSources("Source1", "Source2");
			CreateCacheUsingPackageSources();
			
			IPackageRepository aggregateRepository = cache.CreateAggregateRepository();
			FakePackageRepository expectedRepository = fakePackageRepositoryFactory.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, aggregateRepository);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageSourcesButOneDisabled_ReturnsAggregateRepositoryCreatedWithOnlyEnabledPackageSource()
		{
			CreatePackageSources();
			packageSourcesHelper.AddTwoPackageSources("Source1", "Source2");
			packageSourcesHelper.RegisteredPackageSources[0].IsEnabled = false;
			CreateCacheUsingPackageSources();
			FakePackageRepository repository1 = AddFakePackageRepositoryForPackageSource("Source1");
			FakePackageRepository repository2 = AddFakePackageRepositoryForPackageSource("Source2");
			var expectedRepositories = new FakePackageRepository[] {
				repository2
			};
			
			cache.CreateAggregateRepository();
			
			IEnumerable<IPackageRepository> repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			var actualRepositoriesAsList = new List<IPackageRepository>(repositoriesUsedToCreateAggregateRepository);
			IPackageRepository[] actualRepositories = actualRepositoriesAsList.ToArray();
			
			CollectionAssert.AreEqual(expectedRepositories, actualRepositories);
		}
				
		[Test]
		public void CreateAggregatePackageRepository_TwoRegisteredPackageRepositories_AllRegisteredRepositoriesUsedToCreateAggregateRepositoryFromFactory()
		{
			CreatePackageSources();
			packageSourcesHelper.AddTwoPackageSources("Source1", "Source2");
			CreateCacheUsingPackageSources();
			
			FakePackageRepository repository1 = AddFakePackageRepositoryForPackageSource("Source1");
			FakePackageRepository repository2 = AddFakePackageRepositoryForPackageSource("Source2");
			var expectedRepositories = new FakePackageRepository[] {
				repository1,
				repository2
			};
			
			cache.CreateAggregateRepository();
			
			IEnumerable<IPackageRepository> repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			var actualRepositoriesAsList = new List<IPackageRepository>(repositoriesUsedToCreateAggregateRepository);
			IPackageRepository[] actualRepositories = actualRepositoriesAsList.ToArray();
			
			CollectionAssert.AreEqual(expectedRepositories, actualRepositories);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_OnePackageRepositoryPassed_ReturnsAggregateRepositoryFromFactory()
		{
			CreateCache();
			
			var repositories = new FakePackageRepository[] {
				new FakePackageRepository()
			};
			IPackageRepository aggregateRepository = cache.CreateAggregateRepository(repositories);
			
			FakePackageRepository expectedRepository = fakePackageRepositoryFactory.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, aggregateRepository);
		}
		
		[Test]
		public void CreateAggregatePackageRepository_OnePackageRepositoryPassed_RepositoryUsedToCreateAggregateRepository()
		{
			CreateCache();
			
			var repositories = new FakePackageRepository[] {
				new FakePackageRepository()
			};
			cache.CreateAggregateRepository(repositories);
			
			IEnumerable<IPackageRepository> repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			Assert.AreEqual(repositories, repositoriesUsedToCreateAggregateRepository);
		}
		
		[Test]
		public void RecentPackageRepository_NoRecentPackages_ReturnsRecentRepositoryCreatedByFactory()
		{
			CreateCache();
			IRecentPackageRepository repository = cache.RecentPackageRepository;
			FakeRecentPackageRepository expectedRepository = fakePackageRepositoryFactory.FakeRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void RecentPackageRepository_NoRecentPackages_CreatedWithAggregateRepository()
		{
			CreateCache();
			IRecentPackageRepository repository = cache.RecentPackageRepository;
			
			FakePackageRepository expectedRepository = fakePackageRepositoryFactory.FakeAggregateRepository;
			IPackageRepository actualRepository = fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void RecentPackageRepository_OneRecentPackage_RecentPackageUsedToCreateRecentPackageRepository()
		{
			CreateCache();
			RecentPackageInfo recentPackage = AddOneRecentPackage();
			
			IRecentPackageRepository repository = cache.RecentPackageRepository;
			
			IList<RecentPackageInfo> actualRecentPackages = fakePackageRepositoryFactory.RecentPackagesPassedToCreateRecentPackageRepository;
			
			var expectedRecentPackages = new RecentPackageInfo[] {
				recentPackage
			};
			
			Assert.AreEqual(expectedRecentPackages, actualRecentPackages);
		}
		
		[Test]
		public void RecentPackageRepository_OnePackageSource_OneRepositoryCreatedForPackageSourceAndUsedToCreateAggregateRepository()
		{
			CreatePackageSources();
			packageSourcesHelper.AddOnePackageSource("Source1");
			CreateCacheUsingPackageSources();
			
			FakePackageRepository repository = AddFakePackageRepositoryForPackageSource("Source1");
			var expectedRepositories = new FakePackageRepository[] {
				repository
			};
			
			IRecentPackageRepository recentRepository = cache.RecentPackageRepository;
			
			IEnumerable<IPackageRepository> repositoriesUsedToCreateAggregateRepository = 
				fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository;
			
			var actualRepositoriesAsList = new List<IPackageRepository>(repositoriesUsedToCreateAggregateRepository);
			IPackageRepository[] actualRepositories = actualRepositoriesAsList.ToArray();
			
			CollectionAssert.AreEqual(expectedRepositories, actualRepositories);
		}
		
		[Test]
		public void RecentPackageRepository_PropertyAccessedTwice_AggregateRepositoryCreatedOnce()
		{
			CreateCache();
			IRecentPackageRepository repository = cache.RecentPackageRepository;
			fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository = null;
			repository = cache.RecentPackageRepository;
			
			Assert.IsNull(fakePackageRepositoryFactory.RepositoriesPassedToCreateAggregateRepository);
		}
			
		[Test]
		public void CreateRecentPackageRepository_AggregateRepositoryPassedAndNoRecentPackagesPassed_UsesFactoryToCreateRepository()
		{
			CreateCache();
			IPackageRepository repository = CreateRecentPackageRepositoryPassingAggregateRepository();
			
			FakeRecentPackageRepository expectedRepository = fakePackageRepositoryFactory.FakeRecentPackageRepository;
			
			Assert.AreEqual(expectedRepository, repository);
		}
		
		[Test]
		public void CreateRecentPackageRepository_AggregateRepositoryPassedAndNoRecentPackagesPassed_AggregateIsUsedToCreateRepository()
		{
			CreateCache();
			CreateRecentPackageRepositoryPassingAggregateRepository();
			
			IPackageRepository actualRepository = fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository;
			
			Assert.AreEqual(fakeAggregateRepositoryPassedToCreateRecentPackageRepository, actualRepository);
		}
		
		[Test]
		public void CreateRecentPackageRepository_AggregateRepositoryPassedAndNoRecentPackagesPassed_RecentPackagesUsedToCreateRepository()
		{
			CreateCache();
			CreateRecentPackageRepositoryPassingAggregateRepository();
			
			IList<RecentPackageInfo> recentPackages = fakePackageRepositoryFactory.RecentPackagesPassedToCreateRecentPackageRepository;
			
			Assert.AreEqual(recentPackagesPassedToCreateRecentPackageRepository, recentPackages);
		}
		
		[Test]
		public void CreateRecentPackageRepository_MethodCalledTwice_RecentPackageRepositoryCreatedOnce()
		{
			CreateCache();
			CreateRecentPackageRepositoryPassingAggregateRepository();
			fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository = null;
			CreateRecentPackageRepositoryPassingAggregateRepository();
			
			Assert.IsNull(fakePackageRepositoryFactory.AggregateRepositoryPassedToCreateRecentPackageRepository);
		}
	}
}
