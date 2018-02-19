﻿using System;
using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.Texture
{
    public partial class TextureToolSidebarPanel : UserControl
    {
        public delegate void RandomiseXShiftValuesEventHandler(object sender, int min, int max);
        public delegate void RandomiseYShiftValuesEventHandler(object sender, int min, int max);
        public delegate void TileFitEventHandler(object sender, int tileX, int tileY);

        public event RandomiseXShiftValuesEventHandler RandomiseXShiftValues;
        public event RandomiseYShiftValuesEventHandler RandomiseYShiftValues;
        public event TileFitEventHandler TileFit;

        protected virtual void OnRandomiseXShiftValues(int min, int max)
        {
            RandomiseXShiftValues?.Invoke(this, min, max);
        }

        protected virtual void OnRandomiseYShiftValues(int min, int max)
        {
            RandomiseYShiftValues?.Invoke(this, min, max);
        }

        protected virtual void OnTileFit(int tileX, int tileY)
        {
            TileFit?.Invoke(this, tileX, tileY);
        }

        public TextureToolSidebarPanel()
        {
            InitializeComponent();
        }

        private void RandomShiftXButtonClicked(object sender, EventArgs e)
        {
            OnRandomiseXShiftValues((int) RandomShiftMin.Value, (int) RandomShiftMax.Value);
        }

        private void RandomShiftYButtonClicked(object sender, EventArgs e)
        {
            OnRandomiseYShiftValues((int)RandomShiftMin.Value, (int)RandomShiftMax.Value);
        }

        private void TileFitButtonClicked(object sender, EventArgs e)
        {
            OnTileFit((int) TileFitX.Value, (int) TileFitY.Value);
        }
    }
}
