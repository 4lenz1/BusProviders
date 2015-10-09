﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BlinkyBackground
{
    public sealed class StartupTask : IBackgroundTask
    {
        private readonly int LED_PIN = 5;
        private readonly long TIMER_TICKS_100_NANOS = (long)(10000000 / 2); // every .5 secs
        private ThreadPoolTimer blinkyTimer;
        private int LEDStatus = 0;
        GpioPin pin = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            if (Microsoft.IoT.Lightning.Providers.Provider.IsLightningEnabled)
            {
                Windows.Devices.LowLevelDevicesController.DefaultProvider =  /* set Lightning as the default provider */
                     new Microsoft.IoT.Lightning.Providers.Provider();
            }

            var gpioController = await GpioController.GetDefaultAsync(); /* Get the default GPIO controller on the system */

            pin = gpioController.OpenPin(LED_PIN, GpioSharingMode.Exclusive);
            pin.Write(GpioPinValue.High);

            blinkyTimer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, new TimeSpan(TIMER_TICKS_100_NANOS));
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            if (LEDStatus == 0)
            {
                LEDStatus = 1;
                if (pin != null)
                {
                    pin.Write(GpioPinValue.High);
                }
            }
            else
            {
                LEDStatus = 0;
                if (pin != null)
                {
                    pin.Write(GpioPinValue.Low);
                }
            }
        }

    }
}
