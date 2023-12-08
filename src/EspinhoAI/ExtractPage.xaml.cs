using System.Collections.Generic;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Maui.Controls;

namespace EspinhoAI;

public partial class ExtractPage : ContentPage
{
    ExtractViewModel _vm;
    public ExtractPage(ExtractViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;

        viewModel.PropertyChanged += ViewModelPropertyChanged;

        var gs = new PointerGestureRecognizer();
        gs.PointerMoved += Gs_PointerEntered;
        graphics.GestureRecognizers.Add(gs);

    }

    private void Gs_PointerEntered(object? sender, PointerEventArgs e)
    {
         var pp =   e.GetPosition(graphics);
        if (pp != null &&  _rects.Any(r => r.Bounds.Contains(pp.Value)))
        {
            var s = _rects.FirstOrDefault(r => r.Bounds.Contains(pp.Value));
            System.Diagnostics.Debug.WriteLine(s.Text);
        }
    }

    List<ParagraphAdorner> _rects;
    private void ViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "PdfImage")
        {
            graphics.Drawable = null;
        }
        if (e.PropertyName == "Paragraphs")
        {
            _rects  = new List<ParagraphAdorner>();

            foreach (var item in _vm.Paragraphs)
            {
                var adorners = GetAdorners(item);
                var ww = new ParagraphAdorner(adorners.First(), item.Content);
              
                _rects.Add(ww);
            }
            var geom = new ParagraphsDrawable(_rects);
            graphics.Drawable = geom;

        }
    }

    static IList<RectF> GetAdorners(DocumentParagraph item)
    {
        var rects = new List<RectF>();

        foreach (var boundingRegion in item.BoundingRegions)
        {
            PointF origin = new(boundingRegion.BoundingPolygon.Min(p => p.X), boundingRegion.BoundingPolygon.Min(p => p.Y));
            SizeF size = new(boundingRegion.BoundingPolygon.Max(p => p.X) - origin.X, boundingRegion.BoundingPolygon.Max(p => p.Y) - origin.Y);
            RectF rec = new RectF(origin, size);
            rects.Add(rec);
        }

        return rects;
    }

    record class ParagraphAdorner(RectF Bounds, string Text);

    class ParagraphsDrawable : IDrawable
    {
        
        PointF of;
        SizeF sf;
        List<ParagraphAdorner> rf;

        public ParagraphsDrawable(List<ParagraphAdorner> rects)
        {
            rf = rects;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.DarkBlue;
            canvas.StrokeSize = 4;
            canvas.FillColor = Colors.LightBlue;
            foreach (var item in rf)
            {
                of = item.Bounds.Location;
                sf = item.Bounds.Size;
                canvas.DrawRectangle((float)of.X, (float)of.Y, sf.Width, sf.Height);
            }
        }
          
    }

}
