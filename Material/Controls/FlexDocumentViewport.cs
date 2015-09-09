﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FlexCharts.Controls.Core;
using FlexCharts.Documents;
using FlexCharts.Helpers.DependencyHelpers;
using FlexCharts.Helpers.EventHelpers;

namespace Material.Controls
{
	[TemplatePart(Name = "PART_root", Type = typeof(Viewbox))]
	public class FlexDocumentViewport : FlexControl
	{
		public static readonly RoutedEvent DocumentAddedEvent = EM.Register<FlexDocumentViewport, RoutedEventHandler>(EM.BUBBLE);
		public event RoutedEventHandler DocumentAdded
		{
			add { AddHandler(DocumentAddedEvent, value); }
			remove { RemoveHandler(DocumentAddedEvent, value); }
		}
		public static readonly DependencyProperty ZoomOffsetProperty = DP.Register(
			new Meta<FlexDocumentViewport, double>(.9));
		public double ZoomOffset
		{
			get { return (double)GetValue(ZoomOffsetProperty); }
			set { SetValue(ZoomOffsetProperty, value); }
		}
		public static readonly DependencyProperty VerticalScrollOffsetProperty = DP.Register(
			new Meta<FlexDocumentViewport, double>());
		public double VerticalScrollOffset
		{
			get { return (double)GetValue(VerticalScrollOffsetProperty); }
			set { SetValue(VerticalScrollOffsetProperty, value); }
		}
		public static readonly DependencyProperty DocumentProperty = DP.Register(
			new Meta<FlexDocumentViewport, FlexDocument>(null, DocumentChanged));

		private static void DocumentChanged(FlexDocumentViewport i, DPChangedEventArgs<FlexDocument> e)
		{
			i.VerticalScrollOffset = 0;
			i.ZoomOffset = .9;
			i.RaiseEvent(new RoutedEventArgs(DocumentAddedEvent));
		}


		public FlexDocument Document
		{
			get { return (FlexDocument)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		private Viewbox PART_root;
		private double? lastTarget = null;
		private double? lastZoomTarget = null;

		public FlexDocumentViewport()
		{
			EventManager.RegisterClassHandler(typeof(FlexDocumentViewport), FlexContentControl.ContentChangedEvent,
				new RoutedEventHandler(OnContentChanged));
		}

		private void OnContentChanged(object s, RoutedEventArgs e)
		{

		}

		static FlexDocumentViewport()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FlexDocumentViewport),
				new FrameworkPropertyMetadata(typeof(FlexDocumentViewport)));

		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				if (lastZoomTarget == null)
				{
					lastZoomTarget = ZoomOffset;
				}
				lastZoomTarget = lastZoomTarget + (e.Delta / 2000.0);
				//PART_root.RenderTransformOrigin = getActualRenderingOrigin();
				BeginAnimation(ZoomOffsetProperty,
					new DoubleAnimation(lastZoomTarget.GetValueOrDefault(.9), new Duration(TimeSpan.FromMilliseconds(150))));
			}
			else
			{
				if (lastTarget == null)
				{
					lastTarget = VerticalScrollOffset;
				}
				lastTarget = lastTarget + (e.Delta / 2.0);

				BeginAnimation(VerticalScrollOffsetProperty,
					new DoubleAnimation(lastTarget.GetValueOrDefault(0), new Duration(TimeSpan.FromMilliseconds(150))));
			}




		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			PART_root = GetTemplateChild<Viewbox>(nameof(PART_root));
		}

		public void ViewCodeBehind()
		{
			if (Document == null)
				return;
			Document.RenderTransformOrigin = new Point(.5, .5);
			var rt = new ScaleTransform(1,1,.5,.5);
			Document.RenderTransform = rt;
			rt.BeginAnimation(ScaleTransform.ScaleXProperty,
				new DoubleAnimation(-1, new Duration(TimeSpan.FromMilliseconds(500)))
				{ EasingFunction = new CircleEase() {EasingMode = EasingMode.EaseInOut} });

		}

		public bool HasContent => Document != null;

		//private Point getActualRenderingOrigin()
		//{
		//	var viewportCenter = new Point(ActualWidth / 2, ActualHeight / 2);
		//	var viewportHeight = ActualHeight;
		//	var documentHeight = Document.ActualHeight * Zo;
		//	var midScale = viewportHeight / documentHeight;
		//	var topScale = 1 - midScale;
		//	return new Point(.5, topScale);
		//}
	}
}