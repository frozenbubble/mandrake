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

namespace Mandrake.Client.View
{
    public delegate void CursorHoverEventHandler(object sender, MouseEventArgs e);

    public class ColoredCaret
    {

        private Rectangle head;
        private Rectangle body;

        public event CursorHoverEventHandler Hover;

        public Canvas CursorCanvas { get; private set; }

        private Point position;
        public Point Position 
        {
            get { return position; }
            set
            {
                position = value;
                Canvas.SetLeft(head, value.X);
                Canvas.SetTop(head, value.Y);
                Canvas.SetLeft(body, value.X + 2);
                Canvas.SetTop(body, value.Y);
            }
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int FontSize { get; set; }

        public ColoredCaret(Canvas canvas, Guid id, string name) : this(canvas, new Point(), id, name, 12) { }

        public ColoredCaret(Canvas canvas, Point position, Guid id, string name, int fontSize)
        {
            this.CursorCanvas = canvas;
            this.Position = position;
            this.Id = id;
            this.Name = name;
            this.FontSize = fontSize;
            this.Color = GenerateColor();

            head = new Rectangle();
            head.Height = 5;
            head.Width = 5;
            head.Fill = new SolidColorBrush(Color);

            body = new Rectangle();
            head.Height = FontSize;
            head.Width = 1;
            head.Fill = new SolidColorBrush(Color);

            Canvas.SetLeft(head, 0);
            Canvas.SetTop(head, 0);
            Canvas.SetLeft(body, 2);
            Canvas.SetTop(body, 0);
            canvas.Children.Add(head);
            canvas.Children.Add(body);

            head.MouseEnter += MouseEnter;
            body.MouseEnter += MouseEnter;
        }


        void MouseEnter(object sender, MouseEventArgs e)
        {
            if (Hover != null) Hover(sender, e);
        }

        private Color GenerateColor()
        {
            var random = new Random();

            return new Color()
            {
                R = (byte)random.Next(256),
                G = (byte)random.Next(256),
                B = (byte)random.Next(256)
            };
        }
    }
}
