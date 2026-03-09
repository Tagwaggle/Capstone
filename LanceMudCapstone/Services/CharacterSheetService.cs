using LanceMudCapstone.DTOs;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace LanceMudCapstone.Services;

public class CharacterSheetService
{
    public byte[] GenerateCharacterSheet(PlayerCharacterDto character)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.PageColor(Colors.Grey.Darken4);
                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text($"Name: {character.Name}").FontSize(24).SemiBold().FontColor(Colors.Purple.Lighten2);
                            left.Item().Text($"Race: {character.Race ?? "Unknown"}").FontColor(Colors.White);
                            left.Item().Text($"Class: {character.Class ?? "Unknown"}").FontColor(Colors.White);
                            left.Item().Text($"Level: {character.Level}").FontColor(Colors.White);
                            left.Item().Text($"Alignment: {character.Alignment ?? "Unknown"}").FontColor(Colors.White);
                        });
                        row.RelativeItem().Column(right =>
                        {
                            right.Item().Text($"Created: {character.CreatedAt:yyyy-MM-dd}").FontColor(Colors.Grey.Lighten1);
                            right.Item().Text($"Last Online: {(character.LastOnline.HasValue ? character.LastOnline.Value.ToString("yyyy-MM-dd") : "Never")}").FontColor(Colors.Grey.Lighten1);
                            right.Item().Text($"Gold: {character.Gold:N0} gp").FontColor(Colors.Grey.Lighten1);
                        });
                    });
                    col.Item().Row(RowDescriptor =>
                    {
                        RowDescriptor.RelativeItem().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(stats =>
                        {
                            stats.Item().Text("Attributes").FontSize(18).SemiBold().FontColor(Colors.Purple.Lighten2);
                            stats.Item().Text($"STR: {character.Strength}").FontColor(Colors.White);
                            stats.Item().Text($"DEX: {character.Dexterity}").FontColor(Colors.White);
                            stats.Item().Text($"CON: {character.Constitution}").FontColor(Colors.White);
                            stats.Item().Text($"INT: {character.Intelligence}").FontColor(Colors.White);
                            stats.Item().Text($"WIS: {character.Wisdom}").FontColor(Colors.White);
                            stats.Item().Text($"CHA: {character.Charisma}").FontColor(Colors.White);
                        });
                        RowDescriptor.ConstantItem(10);

                        RowDescriptor.RelativeItem().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(combat =>
                        {
                            combat.Item().Text("Combat").FontSize(18).SemiBold().FontColor(Colors.Purple.Lighten2);
                            combat.Item().Text($"HP: {character.Health}/{character.MaxHealth}").FontColor(Colors.White);
                            combat.Item().Text($"MP: {character.Mana}/{character.MaxMana}").FontColor(Colors.White);
                            combat.Item().Text($"Stamina: {character.Stamina}/{character.MaxStamina}").FontColor(Colors.White);
                            combat.Item().Text($"AC: {character.ArmorClass}").FontColor(Colors.White);
                            combat.Item().Text($"Hit Dice: {character.HitDice ?? "N/A"}").FontColor(Colors.White);
                        });
                        RowDescriptor.ConstantItem(11);
                        RowDescriptor.RelativeItem().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(progression =>
                        {
                            progression.Item().Text("Progression").FontSize(18).SemiBold().FontColor(Colors.Purple.Lighten2);
                            progression.Item().Text($"XP: {character.Xp}/{character.XpNeeded}").FontColor(Colors.White);
                            progression.Item().Text($"Active Quest: {(character.ActiveQuest.HasValue ? $"Quest #{character.ActiveQuest.Value}" : "None")}").FontColor(Colors.White);
                        });
                    });
                    col.Item().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(footer =>
                    {
                        footer.Item().Text($"Legends of the Lance - Character Sheet - Generated at {DateTime.Now:MMM dd, yy HH:mm}").FontSize(12).SemiBold().FontColor(Colors.Purple.Lighten2);
                        footer.Item().Text($" UserName: {character.Username ?? "Unknown"}").FontColor(Colors.Grey.Lighten1);
                    });
                });
            });
        });
                return document.GeneratePdf();
            }
            }





    