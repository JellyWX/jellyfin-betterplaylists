using Jellyfin.Data.Entities;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.BetterPlaylists.QueryEngine;

namespace Jellyfin.Plugin.BetterPlaylists
{
    public class SmartPlaylist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string User { get; set; }
        public List<ExpressionSet> ExpressionSets { get; set; }
        public int MaxItems { get; set; }

        public SmartPlaylist(SmartPlaylistDto dto)
        {
            this.Id = dto.Id;
            this.Name = dto.Name;
            this.FileName = dto.FileName;
            this.User = dto.User;
            this.ExpressionSets = Engine.FixRuleSets(dto.ExpressionSets);
            if (dto.MaxItems > 0)
            {
                this.MaxItems = dto.MaxItems;
            }
            else
            {
                this.MaxItems = 1000;
            }
        }

        private List<List<Func<Operand, bool>>> CompileRuleSets()
        {

            List<List<Func<Operand, bool>>> compiledRuleSets = new List<List<Func<Operand, bool>>>();
            foreach (var set in this.ExpressionSets)
            {
                compiledRuleSets.Add(set.Expressions.Select(r => Engine.CompileRule<Operand>(r)).ToList());
            }

            return compiledRuleSets;
        }

        // Returns the ID's of the items, if order is provided the IDs are sorted.
        public IEnumerable<Guid> FilterPlaylistItems(IEnumerable<BaseItem> items, ILibraryManager libraryManager,
            User user)
        {
            var results = new List<BaseItem> { };

            var compiledRules = CompileRuleSets();
            foreach (var i in items)
            {
                var operand = OperandFactory.GetMediaType(libraryManager, i, user);

                if (compiledRules.Any(set => set.All(rule => rule(operand))))
                {
                    results.Add(i);
                }
            }

            return results.Select(x => x.Id);
        }

        private static void Validate()
        {
            //Todo create validation for constructor
        }
    }
}