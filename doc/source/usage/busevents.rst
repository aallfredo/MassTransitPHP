Observing lifecycle events
==========================

.. attention:: **This page is obsolete!**

   New documentation is located at http://masstransit-project.com/MassTransit.

   The latest version of this page can be found here_.

.. _here: http://masstransit-project.com/MassTransit/usage/lifecycle-observers.html

When integrating a framework into your application, it can be useful to understand when the framework is "doing stuff."
Whether it is starting up, shutting down, or anything in between, being notified and thereby able to take action is a
huge benefit.

MassTransit supports a number of lifecycle events that can be observed, making it easy to build components that are
started and stopped along with the bus.

.. note::

    A good example of this is the ``UseInMemoryMessageScheduler``, which is part of the Quartz.NET integration
    package. Using the lifecycle events, Quartz is able to be started and stopped on a receive endpoint without
    any additional development. And that saves you time.

To observe bus events, create a class which implements ``IBusObserver``, as shown below.

.. sourcecode:: csharp

    public class BusObserver :
        IBusObserver
    {
        public Task PostCreate(IBus bus)
        {
            // called after the bus has been created, but before it has been started.
        }

        public Task CreateFaulted(Exception exception)
        {
            // called if the bus creation fails for some reason
        }

        public Task PreStart(IBus bus)
        {
            // called just before the bus is started
        }

        public Task PostStart(IBus bus, Task busReady)
        {
            // called once the bus has been started successfully. The task can be used to wait for
            // all of the receive endpoints to be ready.
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            // called if the bus fails to start for some reason (dead battery, no fuel, etc.)
        }

        public Task PreStop(IBus bus)
        {
            // called just before the bus is stopped
        }

        public Task PostStop(IBus bus)
        {
            // called after the bus has been stopped
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            // called if the bus fails to stop (no brakes)
        }
    }

Bus observers can only be configured during bus configuration. To connect a bus observer during
bus configuration, refer to the example shown below.

.. sourcecode:: csharp

    var busObserver = new BusObserver();

    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
        {
            e.Consumer<UpdateCustomerConsumer>();
        });

        cfg.BusObserver(busObserver);
    });
