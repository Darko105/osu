﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Game.Online.API.Requests;
using osu.Game.Users;
using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using System.Collections.Generic;
using osu.Game.Online.API;
using osu.Framework.Allocation;

namespace osu.Game.Overlays.Profile.Sections.Ranks
{
    public class PaginatedScoreContainer : PaginatedContainerWithHeader<APILegacyScoreInfo>
    {
        private readonly ScoreType type;

        public PaginatedScoreContainer(ScoreType type, Bindable<User> user, string headerText, CounterVisibilityState counterVisibilityState, string missingText = "")
            : base(user, headerText, counterVisibilityState, missingText)
        {
            this.type = type;

            ItemsPerPage = 5;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            ItemsContainer.Direction = FillDirection.Vertical;
        }

        protected override int GetCount(User user)
        {
            switch (type)
            {
                case ScoreType.Firsts:
                    return user.ScoresFirstCount;

                default:
                    return 0;
            }
        }

        protected override APIRequest<List<APILegacyScoreInfo>> CreateRequest() =>
            new GetUserScoresRequest(User.Value.Id, type, VisiblePages++, ItemsPerPage);

        protected override Drawable CreateDrawableItem(APILegacyScoreInfo model)
        {
            switch (type)
            {
                default:
                    return new DrawableProfileScore(model.CreateScoreInfo(Rulesets));

                case ScoreType.Best:
                    return new DrawableProfileWeightedScore(model.CreateScoreInfo(Rulesets), Math.Pow(0.95, ItemsContainer.Count));
            }
        }
    }
}
