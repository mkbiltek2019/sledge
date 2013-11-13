﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Input;
using Sledge.Graphics;
using Sledge.UI;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Tools;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using Sledge.Graphics.Helpers;
using KeyboardState = Sledge.UI.KeyboardState;

namespace Sledge.Editor.UI
{
    public class Camera3DViewportListener : IViewportEventListener
    {
        public ViewportBase Viewport { get; set; }

        private int LastKnownX { get; set; }
        private int LastKnownY { get; set; }
        private bool PositionKnown { get; set; }
        private bool FreeLook { get; set; }
        private bool CursorVisible { get; set; }
        private bool Focus { get; set; }
        private Camera Camera { get; set; }

        public Camera3DViewportListener(Viewport3D vp)
        {
            LastKnownX = 0;
            LastKnownY = 0;
            PositionKnown = false;
            FreeLook = false;
            CursorVisible = true;
            Focus = false;
            Viewport = vp;
            Camera = vp.Camera;
            _downKeys = new List<Keys>();
        }

        private readonly List<Keys> _downKeys;

        public void UpdateFrame()
        {
            if (!Focus) return;
            var amt = 15m;
            // These keys are used for hotkeys, don't want the 3D view to move about when trying to use hotkeys.
            var ignore = KeyboardState.IsAnyKeyDown(Keys.ShiftKey, Keys.ControlKey, Keys.Alt);
            IfKey(Keys.W, () => Camera.Advance(amt), ignore);
            IfKey(Keys.S, () => Camera.Advance(-amt), ignore);
            IfKey(Keys.A, () => Camera.Strafe(-amt), ignore);
            IfKey(Keys.D, () => Camera.Strafe(amt), ignore);
        }

        private void IfKey(Keys key, Action action, bool ignoreKeyboard)
        {
            if (!KeyboardState.IsKeyDown(key))
            {
                _downKeys.Remove(key);
            }
            else if (ignoreKeyboard)
            {
                if (_downKeys.Contains(key)) action();
            }
            else
            {
                if (!_downKeys.Contains(key)) _downKeys.Add(key);
                action();
            }
        }

        public void PreRender()
        {
            //
        }

        public void Render3D()
        {
            //
        }

        public void Render2D()
        {
            if (!Focus || !FreeLook) return;

            TextureHelper.DisableTexturing();
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.White);
            GL.Vertex2(0, -5);
            GL.Vertex2(0, 5);
            GL.Vertex2(1, -5);
            GL.Vertex2(1, 5);
            GL.Vertex2(-5, 0);
            GL.Vertex2(5, 0);
            GL.Vertex2(-5, 1);
            GL.Vertex2(5, 1);
            GL.End();
            TextureHelper.EnableTexturing();
        }

        public void KeyUp(ViewportEvent e)
        {
            //
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!Focus) return;
            if (e.KeyCode == Keys.Z && !e.Alt && !e.Control && !e.Shift)
            {
                FreeLook = !FreeLook;
                PositionKnown = false;
                if (FreeLook && CursorVisible)
                {
                    CursorVisible = false;
                    Cursor.Hide();
                }
                else if (!FreeLook && !CursorVisible)
                {
                    CursorVisible = true;
                    Cursor.Show();
                }
            }
            if (FreeLook)
            {
                e.Handled = true;
            }
        }

        public void KeyPress(ViewportEvent e)
        {
            if (FreeLook)
            {
                e.Handled = true;
            }
        }

        public void MouseMove(ViewportEvent e)
        {
            if (!Focus) return;
            if (PositionKnown && (FreeLook || KeyboardState.IsKeyDown(Keys.Space) || ToolManager.ActiveTool is CameraTool))
            {
                var dx = LastKnownX - e.X;
                var dy = e.Y - LastKnownY;
                if (dx != 0 || dy != 0)
                {
                    MouseMoved(e, dx, dy);
                    return;
                }
            }
            LastKnownX = e.X;
            LastKnownY = e.Y;
            PositionKnown = true;
        }

        private void MouseMoved(ViewportEvent e, int dx, int dy)
        {
            var space = KeyboardState.IsKeyDown(Keys.Space) || ToolManager.ActiveTool is CameraTool;
            var left = Control.MouseButtons.HasFlag(MouseButtons.Left);
            var right = Control.MouseButtons.HasFlag(MouseButtons.Right);
            var both = left && right;
            if (both) left = right = false;
            var freelook = FreeLook || (space && left);
            var updown = space && right;
            var forwardback = space && both;

            if (freelook)
            {
                // Camera
                var fovdiv = (Viewport.Width / 60m) / 2.5m;
                Camera.Pan(dx / fovdiv);
                Camera.Tilt(dy / fovdiv);
            }
            else if (updown)
            {
                Camera.Strafe(-dx);
                Camera.Ascend(-dy);
            }
            else if (forwardback)
            {
                Camera.Strafe(-dx);
                Camera.Advance(-dy);
            }

            if (FreeLook)
            {
                LastKnownX = Viewport.Width/2;
                LastKnownY = Viewport.Height/2;
                Cursor.Position = Viewport.PointToScreen(new Point(LastKnownX, LastKnownY));
            }
            else
            {
                LastKnownX = e.X;
                LastKnownY = e.Y;
            }
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Focus || (ToolManager.ActiveTool != null && ToolManager.ActiveTool.IsCapturingMouseWheel())) return;
            Camera.Advance((e.Delta / Math.Abs(e.Delta)) * 500);
        }

        public void MouseUp(ViewportEvent e)
        {
            // Nothing.
        }

        public void MouseDown(ViewportEvent e)
        {
            // Nothing.
        }

        public void MouseEnter(ViewportEvent e)
        {
            Focus = true;
        }

        public void MouseLeave(ViewportEvent e)
        {
            if (FreeLook)
            {
                FreeLook = false;
                if (!CursorVisible)
                {
                    CursorVisible = true;
                    Cursor.Show();
                }
            }
            PositionKnown = false;
            Focus = false;
        }
    }
}