using System;

using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace MyTizenOSAPP
{
    internal class Program : NUIApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        void Initialize()
        {
            Window.Default.KeyEvent += OnKeyEvent;

            TextLabel title = new TextLabel("Hello Tizen!")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextColor = Color.Black,
                PointSize = 48.0f,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                WidthResizePolicy = ResizePolicyType.FillToParent
            };
            Window.Default.GetDefaultLayer().Add(title);

            TextLabel Instructions = new TextLabel("Press Back or Escape to Exit")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = Color.Black,
                PointSize = 24.0f,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                WidthResizePolicy = ResizePolicyType.FillToParent
            };
            Window.Default.GetDefaultLayer().Add(Instructions);
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
