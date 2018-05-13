using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaraUi.Animations;
using System.Drawing.Text;
using WaraUi.Properties;

namespace WaraUi.Controls
{
    public class WaraTabSelectorVertical : Control, IWaraControl
    {
        [Browsable(false)]
        public int Depth { get; set; }
        [Browsable(false)]
        public WaraSkinManager SkinManager { get { return WaraSkinManager.Instance; } }
        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private WaraTabControl baseTabControl;
        public WaraTabControl BaseTabControl
        {
            get { return baseTabControl; }
            set
            {
                baseTabControl = value;
                if (baseTabControl == null) return;
                previousSelectedTabIndex = baseTabControl.SelectedIndex;
                baseTabControl.Deselected += (sender, args) =>
                {
                    previousSelectedTabIndex = baseTabControl.SelectedIndex;
                };
                baseTabControl.SelectedIndexChanged += (sender, args) =>
                {
                    animationManager.SetProgress(0);
                    animationManager.StartNewAnimation(AnimationDirection.In);
                };
                baseTabControl.ControlAdded += delegate
                {
                    Invalidate();
                };
                baseTabControl.ControlRemoved += delegate
                {
                    Invalidate();
                };
            }
        }

        private int previousSelectedTabIndex;
        private Point animationSource;
        private readonly AnimationManager animationManager;

        private List<Rectangle> tabRects;
        private const int TAB_HEADER_PADDING_BOTTOM = 32;
        private const int TAB_HEADER_PADDING = 24;
        private const int TAB_INDICATOR_HEIGHT = 2;

        private Font font;

        public WaraTabSelectorVertical()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            Height = 200;
            Width = 400;
            font = new Font(SkinManager.LoadFont(Resources.Roboto_Medium), 14f);

            animationManager = new AnimationManager
            {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.06
            };
            animationManager.OnAnimationProgress += sender => Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.Clear(SkinManager.ColorScheme.PrimaryColor);

            if (baseTabControl == null) return;

            if (!animationManager.IsAnimating() || tabRects == null || tabRects.Count != baseTabControl.TabCount)
                UpdateTabRects();

            double animationProgress = animationManager.GetProgress();

            //Click feedback
            if (animationManager.IsAnimating())
            {
                try
                {
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationProgress * 50)), Color.White));
                    var rippleSize = (int)(animationProgress * tabRects[baseTabControl.SelectedIndex].Width * 1.75);

                    g.SetClip(tabRects[baseTabControl.SelectedIndex]);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                    g.ResetClip();
                    rippleBrush.Dispose();
                }
                catch
                {
                }
            }

            //Draw tab headers
            foreach (TabPage tabPage in baseTabControl.TabPages)
            {
                int currentTabIndex = baseTabControl.TabPages.IndexOf(tabPage);
                Brush textBrush = new SolidBrush(Color.FromArgb(CalculateTextAlpha(currentTabIndex, animationProgress), SkinManager.ColorScheme.TextColor));

                g.DrawString(
                    tabPage.Text.ToUpper(),
                    new Font(SkinManager.LoadFont(Resources.Roboto_Medium), 14f),
                    textBrush,
                    tabRects[currentTabIndex],
                    new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near }
                );
                textBrush.Dispose();
            }

            //Animate tab indicator
            int previousSelectedTabIndexIfHasOne = previousSelectedTabIndex == -1 ? baseTabControl.SelectedIndex : previousSelectedTabIndex;
            if (previousSelectedTabIndexIfHasOne > -1)
            {
                try
                {
                    Rectangle previousActiveTabRect = tabRects[previousSelectedTabIndexIfHasOne];
                    Rectangle activeTabPageRect = tabRects[baseTabControl.SelectedIndex];


                    //Inverser car veticale
                    int y = previousActiveTabRect.Y + (int)((activeTabPageRect.Y - previousActiveTabRect.Y) * animationProgress) + TAB_HEADER_PADDING;
                    int x = activeTabPageRect.X - 2;

                    int width = this.Width - 10;//previousActiveTabRect.Width + (int)((activeTabPageRect.Width - previousActiveTabRect.Width) * animationProgress);

                    g.FillRectangle(SkinManager.ColorScheme.AccentBrush, x, y, width, TAB_INDICATOR_HEIGHT);
                }
                catch
                {
                }
            }
        }

        private int CalculateTextAlpha(int tabIndex, double animationProgress)
        {
            int primaryA = SkinManager.ACTION_BAR_TEXT.A;
            int secondaryA = SkinManager.ACTION_BAR_TEXT_SECONDARY.A;

            if (tabIndex == baseTabControl.SelectedIndex && !animationManager.IsAnimating())
            {
                return primaryA;
            }
            if (tabIndex != previousSelectedTabIndex && tabIndex != baseTabControl.SelectedIndex)
            {
                return secondaryA;
            }
            if (tabIndex == previousSelectedTabIndex)
            {
                return primaryA - (int)((primaryA - secondaryA) * animationProgress);
            }
            return secondaryA + (int)((primaryA - secondaryA) * animationProgress);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (tabRects == null)
            {
                UpdateTabRects();
            }
            for (int i = 0; i < tabRects.Count; i++)
            {
                if (tabRects[i].Contains(e.Location))
                {
                    baseTabControl.SelectedIndex = i;
                }
            }

            animationSource = e.Location;
        }

        private void UpdateTabRects()
        {
            tabRects = new List<Rectangle>();

            // S'il n'y a pas de contrôle d'onglet de base, les rects ne doivent pas être calculés
            // S'il n'y a pas de pages d'onglets dans le contrôle de l'onglet de base, la liste devrait être vide et déjà définie; Sortir du vide
            if (baseTabControl == null || baseTabControl.TabCount == 0) return;

            //Calculez les limites de chaque en-tête de tabulation spécifié dans le contrôle d'onglet de base
            using (var b = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(b))
                {
                    //Premiere élément
                    tabRects.Add(new Rectangle(
                        5, 
                        SkinManager.FORM_PADDING,
                        TAB_HEADER_PADDING * 2 + (int)g.MeasureString(baseTabControl.TabPages[0].Text, font).Width, 
                        Height
                    ));
                    
                    //Element suivant
                    for (int i = 1; i < baseTabControl.TabPages.Count; i++)
                    {
                        tabRects.Add(new Rectangle(
                                5,
                                tabRects[i - 1].Top + TAB_HEADER_PADDING_BOTTOM,
                                TAB_HEADER_PADDING * 2 + (int)g.MeasureString(baseTabControl.TabPages[i].Text, font).Width, 
                                Height
                            )
                        );
                    }
                }
            }
        }
    }
}
