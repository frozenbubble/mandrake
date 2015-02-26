﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mandrake.Client.View
{
    public delegate void CursorHoverEventHandler(object sender, MouseEventArgs e);

    public abstract class ColoredItem
    {
        public Canvas PaintingCanvas { get; protected set; }
        public Guid Id { get; set; }
        public Color Color { get; set; }

        public static Color GenerateColor(Guid id)
        {
            var bytes = BitConverter.GetBytes(id.GetHashCode());
            return Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
        }
    }

    public class ColoredCaret: ColoredItem
    {

        private Rectangle head;
        private Rectangle body;

        public event CursorHoverEventHandler Hover;

        private Point position;
        public Point Position 
        {
            get { return position; }
            set
            {
                //Action emptyDelegate = delegate { };

                position = value;
                Canvas.SetLeft(head, value.X);
                Canvas.SetTop(head, value.Y);
                Canvas.SetLeft(body, value.X + 2);
                Canvas.SetTop(body, value.Y);

                //CursorCanvas.Dispatcher.Invoke(emptyDelegate, DispatcherPriority.Render);

                PaintingCanvas.InvalidateVisual();
            }
        }
        public string Name { get; set; }
        public int FontSize { get; set; }

        public ColoredCaret(Canvas canvas, Guid id, string name) : this(canvas, new Point(), id, name, 12) { }

        public ColoredCaret(Canvas canvas, Point position, Guid id, string name, int fontSize)
        {
            this.Color = GenerateColor(id);
            this.PaintingCanvas = canvas;
            this.Id = id;
            this.Name = name;
            this.FontSize = fontSize;

            head = new Rectangle();
            head.Height = 5;
            head.Width = 5;
            head.Fill = new SolidColorBrush(Color);

            body = new Rectangle();
            body.Height = FontSize;
            body.Width = 1;
            body.Fill = new SolidColorBrush(Color);

            canvas.Children.Add(body);
            canvas.Children.Add(head);
            
            this.Position = position;
        }


        void MouseEnter(object sender, MouseEventArgs e)
        {
            if (Hover != null) Hover(sender, e);
        }
    }


    public class LineSelection
    {
        public Rectangle Box { get; set; }
        public Point Position { get; set; }

        public LineSelection(int width, Point position, Color color)
        {
            Box = new Rectangle();
            Box.Height = 12;
            Box.Width = width;
            Box.Fill = new SolidColorBrush(color);

            Position = position;
        }
    }

    public class ColoredSelection: ColoredItem
    {
        public ColoredSelection(Guid id, Canvas canvas)
        {
            this.Id = id;
            this.Color = GenerateColor(id);
            this.PaintingCanvas = canvas;
        }

        private List<LineSelection> lines;
        public List<LineSelection> Lines 
        {
            get { return lines; }
            set
            {
                if (lines != null)
                {
                    foreach (var item in lines)
                    {
                        PaintingCanvas.Children.Remove(item.Box);
                    }
                }

                foreach (var line in value)
                {
                    Canvas.SetLeft(line.Box, line.Position.X);
                    Canvas.SetTop(line.Box, line.Position.Y);
                    PaintingCanvas.Children.Add(line.Box);
                }

                lines = value;
            }
        }
    }
}
