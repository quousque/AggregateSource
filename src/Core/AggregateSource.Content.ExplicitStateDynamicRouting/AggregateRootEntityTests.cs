﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource
{
    namespace AggregateRootEntityTests
    {
        [TestFixture]
        public class WithAnyInstance
        {
            [Test]
            public void IsAggregateRootEntity()
            {
                Assert.IsInstanceOf<IAggregateRootEntity>(new AnyAggregateRootEntity());
            }

            [Test]
            public void InitializeEventsCanNotBeNull()
            {
                Assert.Throws<ArgumentNullException>(() => new InitializeWithNullEventsAggregateRootEntity());
            }

            [Test]
            public void ApplyEventCanNotBeNull()
            {
                var sut = new ApplyNullEventAggregateRootEntity();
                Assert.Throws<ArgumentNullException>(sut.ApplyNull);
            }

            [Test]
            public void ApplyCanBeInterceptedBeforeApplication()
            {
                var sut = new ApplyInterceptorAggregateRootEntity();
                Assert.That(sut.BeforeApplyWasCalled, Is.True);
            }

            [Test]
            public void ApplyCanBeInterceptedAfterApplication()
            {
                var sut = new ApplyInterceptorAggregateRootEntity();
                Assert.That(sut.AfterApplyWasCalled, Is.True);
            }
        }

        class AnyState
        {
            public void When(object @event) { }
        }

        class AnyAggregateRootEntity : AggregateRootEntity<AnyState> { }

        class InitializeWithNullEventsAggregateRootEntity : AggregateRootEntity<AnyState>
        {
            public InitializeWithNullEventsAggregateRootEntity()
            {
                Initialize(null);
            }
        }

        class ApplyNullEventAggregateRootEntity : AggregateRootEntity<AnyState>
        {
            public void ApplyNull()
            {
                Apply(null);
            }
        }

        class ApplyInterceptorAggregateRootEntity : AggregateRootEntity<AnyState>
        {
            public ApplyInterceptorAggregateRootEntity()
            {
                Apply(new object());
            }

            protected override void BeforeApply(object @event)
            {
                BeforeApplyWasCalled = true;
            }

            public bool BeforeApplyWasCalled { get; private set; }

            protected override void AfterApply(object @event)
            {
                AfterApplyWasCalled = true;
            }

            public bool AfterApplyWasCalled { get; private set; }
        }

        [TestFixture]
        public class WithPristineInstance
        {
            AggregateRootEntity<PristineState> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new PristineAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] { new object(), new object(), new object() }));
            }
        }

        class PristineState
        {
            public void When(object @event) { }
        }

        class PristineAggregateRootEntity : AggregateRootEntity<PristineState> { }

        [TestFixture]
        public class WithInitializedInstance
        {
            AggregateRootEntity<InitializedState> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new InitializedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] { new object(), new object(), new object() }));
            }
        }

        class InitializedState
        {
            public void When(object @event) { }
        }

        class InitializedAggregateRootEntity : AggregateRootEntity<InitializedState>
        {
            public InitializedAggregateRootEntity()
            {
                Initialize(new[] { new object(), new object() });
            }
        }

        [TestFixture]
        public class WithChangedInstance
        {
            AggregateRootEntity<ChangedState> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new ChangedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.True);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(ChangedAggregateRootEntity.AppliedChanges));
            }

            [Test]
            public void InitializeThrows()
            {
                Assert.Throws<InvalidOperationException>(
                    () => _sut.Initialize(new[] { new object(), new object(), new object() }));
            }
        }

        class ChangedState
        {
            public void When(object @event) { }
        }

        class ChangedAggregateRootEntity : AggregateRootEntity<ChangedState>
        {
            public static readonly object[] AppliedChanges = new[] { new object(), new object() };

            public ChangedAggregateRootEntity()
            {
                foreach (var change in AppliedChanges)
                {
                    Apply(change);
                }
            }
        }

        [TestFixture]
        public class WithInitializedThenChangedInstance
        {
            AggregateRootEntity<InitializedThenChangedState> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new InitializedThenChangedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.True);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(InitializedThenChangedAggregateRootEntity.AppliedChanges));
            }

            [Test]
            public void InitializeThrows()
            {
                Assert.Throws<InvalidOperationException>(
                    () => _sut.Initialize(new[] { new object(), new object(), new object() }));
            }
        }

        class InitializedThenChangedState
        {
            public void When(object @event) { }
        }

        class InitializedThenChangedAggregateRootEntity : AggregateRootEntity<InitializedThenChangedState>
        {
            public static readonly object[] AppliedChanges = new[] { new object(), new object() };

            public InitializedThenChangedAggregateRootEntity()
            {
                Initialize(new[] { new object(), new object() });
                foreach (var change in AppliedChanges)
                {
                    Apply(change);
                }
            }
        }

        [TestFixture]
        public class WithChangedThenClearedInstance
        {
            AggregateRootEntity<ChangedThenClearedState> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new ChangedThenClearedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] { new object(), new object(), new object() }));
            }
        }

        class ChangedThenClearedState
        {
            public void When(object @event) { }
        }

        class ChangedThenClearedAggregateRootEntity : AggregateRootEntity<ChangedThenClearedState>
        {
            public ChangedThenClearedAggregateRootEntity()
            {
                foreach (var change in new[] { new object(), new object() })
                {
                    Apply(change);
                }
                ClearChanges();
            }
        }

        [TestFixture]
        public class WithInitializedThenChangedThenClearedInstance
        {
            AggregateRootEntity<InitializedThenChangedThenClearedState> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new InitializedThenChangedThenClearedAggregateRootEntity();
            }

            [Test]
            public void ClearChangesDoesNothing()
            {
                Assert.DoesNotThrow(() => _sut.ClearChanges());
            }

            [Test]
            public void HasChangesReturnsFalse()
            {
                Assert.That(_sut.HasChanges(), Is.False);
            }

            [Test]
            public void GetChangesReturnsEmpty()
            {
                Assert.That(_sut.GetChanges(), Is.EquivalentTo(Enumerable.Empty<object>()));
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] { new object(), new object(), new object() }));
            }
        }

        class InitializedThenChangedThenClearedState
        {
            public void When(object @event) { }
        }

        class InitializedThenChangedThenClearedAggregateRootEntity : AggregateRootEntity<InitializedThenChangedThenClearedState>
        {
            public InitializedThenChangedThenClearedAggregateRootEntity()
            {
                Initialize(new[] { new object(), new object() });
                foreach (var change in new[] { new object(), new object() })
                {
                    Apply(change);
                }
                ClearChanges();
            }
        }

        [TestFixture]
        public class WithInstanceWithHandlers
        {
            WithHandlersAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithHandlersAggregateRootEntity();
            }

            [Test]
            public void InitializeCallsHandlerForEachEvent()
            {
                var expectedEvents = new[] { new object(), new object() };

                _sut.Initialize(expectedEvents);

                Assert.That(_sut.PublicState.HandlerCallCount, Is.EqualTo(2));
                Assert.That(_sut.PublicState.RoutedEvents, Is.EquivalentTo(expectedEvents));
            }

            [Test]
            public void ApplyEventCallsEventHandler()
            {
                var @event = new object();

                _sut.DoApply(@event);

                Assert.That(_sut.PublicState.HandlerCallCount, Is.EqualTo(1));
                Assert.That(_sut.PublicState.RoutedEvents, Is.EquivalentTo(new[] { @event }));
            }
        }

        class WithHandlersState
        {
            public WithHandlersState()
            {
                RoutedEvents = new List<object>();
            }

            public void When(object @event)
            {
                HandlerCallCount++;
                RoutedEvents.Add(@event);
            }

            public int HandlerCallCount { get; private set; }
            public List<object> RoutedEvents { get; private set; }
        }

        class WithHandlersAggregateRootEntity : AggregateRootEntity<WithHandlersState>
        {
            public void DoApply(object @event)
            {
                Apply(@event);
            }

            public WithHandlersState PublicState
            {
                get { return State; }
            }
        }

        [TestFixture]
        public class WithInstanceWithoutHandlers
        {
            WithoutHandlersAggregateRootEntity _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = new WithoutHandlersAggregateRootEntity();
            }

            [Test]
            public void InitializeDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.Initialize(new[] { new object(), new object() }));
            }

            [Test]
            public void ApplyEventDoesNotThrow()
            {
                Assert.DoesNotThrow(() => _sut.DoApply(new object()));
            }
        }

        class WithoutHandlersState
        {
            public void When(object @event) { }
        }

        class WithoutHandlersAggregateRootEntity : AggregateRootEntity<WithoutHandlersState>
        {
            public void DoApply(object @event)
            {
                Apply(@event);
            }
        }
    }
}