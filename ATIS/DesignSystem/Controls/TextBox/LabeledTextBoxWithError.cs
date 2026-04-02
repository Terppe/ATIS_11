using System.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace ATIS.DesignSystem.Controls.TextBox
{
    public sealed class LabeledTextBoxWithError : Control
    {
        public LabeledTextBoxWithError()
        {
            DefaultStyleKey = typeof(LabeledTextBoxWithError);
        }

        private TextBlock _errorBlock;
        private Border _border;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            System.Diagnostics.Debug.WriteLine("TEMPLATE WIRD GELADEN");
            System.Diagnostics.Debug.WriteLine($"IsReadOnly DP VALUE: {IsReadOnly}");

            _errorBlock = GetTemplateChild("ErrorBlock") as TextBlock;
            _border = GetTemplateChild("BorderElement") as Border;

            var input = GetTemplateChild("InputElement") as Microsoft.UI.Xaml.Controls.TextBox;
            if (input != null)
            {
                input.TextChanged += (s, e) =>
                {
                    Text = input.Text;
                };

                input.Text = Text;
            }

            UpdateValidationState();
        }

        private void ValidationSource_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
                UpdateValidationState();
        }

        private void UpdateValidationState()
        {
            if (_errorBlock == null || _border == null || ValidationSource == null)
                return;

            var errors = ValidationSource.GetErrors(PropertyName)?.Cast<object>().ToList() ?? new();

            ErrorText = string.Join("\n", errors);
            _errorBlock.Visibility = errors.Any() ? Visibility.Visible : Visibility.Collapsed;

            _border.BorderBrush = errors.Any()
                ? new SolidColorBrush(Colors.Red)
                : BoxBorderBrush;
        }

        // Dependency Properties
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(LabeledTextBoxWithError),
                new PropertyMetadata("", OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LabeledTextBoxWithError)d;

            var input = control.GetTemplateChild("InputElement") as Microsoft.UI.Xaml.Controls.TextBox;
            if (input != null && input.Text != (string)e.NewValue)
            {
                input.Text = (string)e.NewValue;
            }
        }

        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register(nameof(ErrorText), typeof(string), typeof(LabeledTextBoxWithError), new PropertyMetadata(""));




        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(LabeledTextBoxWithError), new PropertyMetadata(""));

        public INotifyDataErrorInfo ValidationSource
        {
            get => (INotifyDataErrorInfo)GetValue(ValidationSourceProperty);
            set => SetValue(ValidationSourceProperty, value);
        }
        public static readonly DependencyProperty ValidationSourceProperty = DependencyProperty.Register(nameof(ValidationSource), typeof(INotifyDataErrorInfo), typeof(LabeledTextBoxWithError),
            new PropertyMetadata(null, OnValidationSourceChanged));

        private static void OnValidationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LabeledTextBoxWithError)d;
            if (e.OldValue is INotifyDataErrorInfo oldSource)
                oldSource.ErrorsChanged -= control.ValidationSource_ErrorsChanged;
            if (e.NewValue is INotifyDataErrorInfo newSource)
                newSource.ErrorsChanged += control.ValidationSource_ErrorsChanged;
            control.UpdateValidationState();
        }
        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(LabeledTextBoxWithError), new PropertyMetadata(""));


        public Brush BoxBackground
        {
            get => (Brush)GetValue(BoxBackgroundProperty);
            set => SetValue(BoxBackgroundProperty, value);
        }
        public static readonly DependencyProperty BoxBackgroundProperty = DependencyProperty.Register(nameof(BoxBackground), typeof(Brush), typeof(LabeledTextBoxWithError),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));
        public Brush BoxBorderBrush
        {
            get => (Brush)GetValue(BoxBorderBrushProperty);
            set => SetValue(BoxBorderBrushProperty, value);
        }
        public static readonly DependencyProperty BoxBorderBrushProperty = DependencyProperty.Register(nameof(BoxBorderBrush), typeof(Brush), typeof(LabeledTextBoxWithError),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        public Thickness BoxBorderThickness
        {
            get => (Thickness)GetValue(BoxBorderThicknessProperty);
            set => SetValue(BoxBorderThicknessProperty, value);
        }
        public static readonly DependencyProperty BoxBorderThicknessProperty = DependencyProperty.Register(nameof(BoxBorderThickness), typeof(Thickness), typeof(LabeledTextBoxWithError),
            new PropertyMetadata(new Thickness(1)));
        public CornerRadius BoxCornerRadius
        {
            get => (CornerRadius)GetValue(BoxCornerRadiusProperty);
            set => SetValue(BoxCornerRadiusProperty, value);
        }
        public static readonly DependencyProperty BoxCornerRadiusProperty = DependencyProperty.Register(nameof(BoxCornerRadius), typeof(CornerRadius), typeof(LabeledTextBoxWithError), new PropertyMetadata(new CornerRadius(6)));

        public Style InnerTextBoxStyle
        {
            get => (Style)GetValue(InnerTextBoxStyleProperty);
            set => SetValue(InnerTextBoxStyleProperty, value);
        }

        public static readonly DependencyProperty InnerTextBoxStyleProperty =
            DependencyProperty.Register(nameof(InnerTextBoxStyle), typeof(Style), typeof(LabeledTextBoxWithError), new PropertyMetadata(null));

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(LabeledTextBoxWithError), new PropertyMetadata(0));

        public void FocusInput()
        {
            var input = GetTemplateChild("InputElement") as Microsoft.UI.Xaml.Controls.TextBox;
            input?.Focus(FocusState.Programmatic);
        }
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(LabeledTextBoxWithError),
                new PropertyMetadata(false));

        //public Brush Background
        //{
        //    get => (Brush)GetValue(BackgroundProperty);
        //    set => SetValue(BackgroundProperty, value);
        //}
        //public static readonly DependencyProperty BackgroundProperty =
        //    DependencyProperty.Register(
        //        nameof(Background),
        //        typeof(Brush),
        //        typeof(LabeledTextBoxWithError),
        //        new PropertyMetadata(null));

        //public Brush BorderBrush
        //{
        //    get => (Brush)GetValue(BorderBrushProperty);
        //    set => SetValue(BorderBrushProperty, value);
        //}
        //public static readonly DependencyProperty BorderBrushProperty =
        //    DependencyProperty.Register(
        //        nameof(BorderBrush),
        //        typeof(Brush),
        //        typeof(LabeledTextBoxWithError),
        //        new PropertyMetadata(null));

        //public Thickness BorderThickness
        //{
        //    get => (Thickness)GetValue(BorderThicknessProperty);
        //    set => SetValue(BorderThicknessProperty, value);
        //}
        //public static readonly DependencyProperty BorderThicknessProperty =
        //    DependencyProperty.Register(
        //        nameof(BorderThickness),
        //        typeof(Thickness),
        //        typeof(LabeledTextBoxWithError),
        //        new PropertyMetadata(new Thickness(0)));

        //public CornerRadius CornerRadius
        //{
        //    get => (CornerRadius)GetValue(CornerRadiusProperty);
        //    set => SetValue(CornerRadiusProperty, value);
        //}
        //public static readonly DependencyProperty CornerRadiusProperty =
        //    DependencyProperty.Register(
        //        nameof(CornerRadius),
        //        typeof(CornerRadius),
        //        typeof(LabeledTextBoxWithError),
        //        new PropertyMetadata(new CornerRadius(0)));

        //public Thickness Padding
        //{
        //    get => (Thickness)GetValue(PaddingProperty);
        //    set => SetValue(PaddingProperty, value);
        //}
        //public static readonly DependencyProperty PaddingProperty =
        //    DependencyProperty.Register(
        //        nameof(Padding),
        //        typeof(Thickness),
        //        typeof(LabeledTextBoxWithError),
        //        new PropertyMetadata(new Thickness(0)));

    }
}
