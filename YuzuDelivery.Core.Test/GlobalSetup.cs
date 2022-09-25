using System;
using System.Collections.Generic;
using System.Threading;
using NSubstitute.Core;
using NSubstitute.Core.DependencyInjection;
using NSubstitute.Routing.AutoValues;

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AccessToModifiedClosure
// ReSharper disable ConvertToLocalFunction
#pragma warning disable CA1050

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void BeforeTests()
    {
        var container = NSubstituteDefaultFactory.DefaultContainer.Customize();
        container.RegisterPerScope<IAutoValueProvidersFactory, CustomAutoProvidersFactory>();

        SubstitutionContext.Current = container.Resolve<ISubstitutionContext>();
    }
}

/// <remarks>
/// <para>
/// NSubstitute has some wonderful tricks to make authoring tests quick and easy. <br/>
/// See example interface below.
/// </para>
///
/// <example>
/// <code>
/// public interface IFoo
/// {
///     IBar GetBar();
/// }
/// </code>
/// </example>
///
/// <para>
/// If we ask for an instance of IFoo we will get one, and if we call GetBar() on that instance it will magically
/// return an instance of IBar.
/// </para>
///
/// <para>
/// If we want GetBar() to behave otherwise, e.g. return null that's not a problem just configure the mock to do so.
/// </para>
///
/// <para>
/// This is all well and good when authoring tests, but quite the PITA when migrating from Rhino Mocks which does not
/// have this behaviour which can result in some other code path being taken unexpectedly when running a test.
/// </para>
///
/// <para>
/// Disabling this functionality for now gets us a smoother migration but ideally we should re-enable the defaults in
/// the future and fix the broken tests accordingly.
/// </para>
/// </remarks>
public class CustomAutoProvidersFactory : IAutoValueProvidersFactory
{
    public IReadOnlyCollection<IAutoValueProvider> CreateProviders(ISubstituteFactory substituteFactory)
    {
        IAutoValueProvider[] result = null;

        var valueFactory = () => result ?? throw new InvalidOperationException("Value was not constructed yet.");
        var lazyResult = new Lazy<IReadOnlyCollection<IAutoValueProvider>>(valueFactory, LazyThreadSafetyMode.PublicationOnly);

        result = new IAutoValueProvider[]
        {
            new AutoObservableProvider(lazyResult),
            new AutoQueryableProvider(),
            // new AutoSubstituteProvider(substituteFactory),
            // new AutoStringProvider(),
            new AutoArrayProvider(),
            new AutoTaskProvider(lazyResult)
        };

        return result;
    }
}

#pragma warning restore CA1050
