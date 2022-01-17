// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Globalization;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Difficulty;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Ranking.Expanded.Statistics
{
    public class PerformanceStatisticTooltip : VisibilityContainer, ITooltip<PerformanceAttributes>
    {
        private readonly Box background;
        private Colour4 totalColour;
        private Colour4 textColour;

        protected override Container<Drawable> Content { get; }

        public PerformanceStatisticTooltip()
        {
            AutoSizeAxes = Axes.Both;
            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both
                },
                Content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Left = 10, Right = 10, Top = 5, Bottom = 5 }
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            background.Colour = colours.Gray3;
            totalColour = colours.Blue;
            textColour = colours.BlueLighter;
        }

        protected override void PopIn()
        {
            // Don't display the tooltip if "Total" is the only item
            if (lastAttributes.GetAttributesForDisplay().Count() > 1)
                this.FadeIn(200, Easing.OutQuint);
        }

        protected override void PopOut() => this.FadeOut(200, Easing.OutQuint);

        private PerformanceAttributes lastAttributes;

        public void SetContent(PerformanceAttributes attributes)
        {
            if (attributes == lastAttributes)
                return;

            lastAttributes = attributes;

            UpdateDisplay(attributes);
        }

        private Drawable createAttributeItem(PerformanceDisplayAttribute attribute, double attributeSum)
        {
            bool isTotal = attribute.PropertyName == nameof(PerformanceAttributes.Total);
            return new GridContainer
            {
                AutoSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 110),
                    new Dimension(GridSizeMode.Absolute, 140),
                    new Dimension(GridSizeMode.AutoSize)
                },
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            Font = OsuFont.GetFont(weight: FontWeight.Regular),
                            Text = attribute.DisplayName,
                            Colour = isTotal ? totalColour : textColour
                        },
                        new Bar
                        {
                            Alpha = isTotal ? 0 : 1,
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            Width = 130,
                            Height = 5,
                            BackgroundColour = Color4.White.Opacity(0.5f),
                            Length = (float)(attribute.Value / attributeSum),
                            Margin = new MarginPadding { Left = 5, Right = 5 }
                        },
                        new OsuSpriteText
                        {
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold),
                            Text = ((int)Math.Round(attribute.Value, MidpointRounding.AwayFromZero)).ToString(CultureInfo.CurrentCulture),
                            Colour = isTotal ? totalColour : textColour
                        }
                    }
                }
            };
        }

        protected virtual void UpdateDisplay(PerformanceAttributes attributes)
        {
            Content.Clear();

            var displayAttributes = attributes.GetAttributesForDisplay();

            double attributeSum = displayAttributes
                                  .Where(attr => attr.PropertyName != nameof(PerformanceAttributes.Total))
                                  .Sum(attr => attr.Value);

            foreach (PerformanceDisplayAttribute attr in displayAttributes)
            {
                Content.Add(createAttributeItem(attr, attributeSum));
            }
        }

        public void Move(Vector2 pos) => Position = pos;
    }
}
