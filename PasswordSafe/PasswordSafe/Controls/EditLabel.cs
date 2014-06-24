using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;

namespace PasswordSafe.Controls
{
    public class EditLabel : Control
    {
        public static readonly RoutedCommand EditCommand = new RoutedCommand("Edit", typeof(EditLabel),
             new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.F2, ModifierKeys.None, "F2") }));

        static EditLabel()
        {
            CommandManager.RegisterClassCommandBinding(typeof(MenuItem), new CommandBinding(EditCommand, OnEditLabel));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditLabel), new FrameworkPropertyMetadata(typeof(EditLabel)));
        }

        public EditLabel()
            : base()
        {
        }


        /// <summary>
        /// This is a very special command execution that expects the sender to be a MenuItem that is a direct child of ContextMenu which again
        /// belongs to an EditLabel.
        /// </summary>
       private static void OnEditLabel(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {

                EditLabel label = (item.Parent as ContextMenu).PlacementTarget as EditLabel;
                if (label != null)
                {
                    label.EditMode = true;
                    label.SelectAll();
                }
            }
        }

        public void SelectAll()
        {
            TextBox.Focus();
            TextBox.SelectAll();
        }


        public object Text
        {
            get { return (object)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(object), typeof(EditLabel), new UIPropertyMetadata(null));




        public bool EditMode
        {
            get { return (bool)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(bool), typeof(EditLabel), new FrameworkPropertyMetadata(false, OnEditModePropertyChanged));


        private static void OnEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (newValue) (d as EditLabel).FocusTextBox(); else (d as EditLabel).UnfocusTextBox();
        }

        private void UnfocusTextBox()
        {
        }

        private void FocusTextBox()
        {
            TextBox.Focus();
            TextBox.SelectAll();
            TextBox.Focus();
        }

        public override void OnApplyTemplate()
        {
            TextBox.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
            base.OnApplyTemplate();
        }

        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EditMode = false;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            EditMode = false;
            base.OnLostKeyboardFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            EditMode = false;
        }


        private TextBox TextBox { get { return GetTemplateChild("PART_TextBox") as TextBox; } }



        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            EditMode = true;
            e.Handled = true;
            base.OnMouseDoubleClick(e);
            TextBox.Focus();
            TextBox.SelectAll();

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //  if (EditMode)
            {
                base.OnMouseDown(e);
            }
        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    if (!EditMode)
                    {
                        e.Handled = true;
                        EditMode = true;
                        //    FocusTextBox();
                    }
                    break;

                case Key.Return:
                    EditMode = false;
                    e.Handled = true;
                    break;

                case Key.Escape:
                    TextBox.Undo();
                    EditMode = false;
                    e.Handled = true;
                    break;
            }
            base.OnKeyDown(e);
        }

    }
}
