using LanceMudCapstone.DTOs;
using LanceMudCapstone.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace LanceMudCapstone.Services;

public class CharacterSheetService
{
    public byte[] GenerateCharacterSheet(PlayerCharacterDto character)
    {
        int statbonus = 0;

        statbonus += (character.Constitution - 10) / 2;

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
                            combat.Item().Text($"Hit Dice: {character.HitDice ?? $"1d6 + {statbonus}"}").FontColor(Colors.White);
                        });
                        RowDescriptor.ConstantItem(11);
                        RowDescriptor.RelativeItem().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(progression =>
                        {
                            progression.Item().Text("Progression").FontSize(18).SemiBold().FontColor(Colors.Purple.Lighten2);
                            progression.Item().Text($"XP: {character.Xp}/{character.XpNeeded}").FontColor(Colors.White);
                            progression.Item().Text($"Active Quest: {(character.ActiveQuest.HasValue ? $"Quest #{character.ActiveQuest.Value}" : "None")}").FontColor(Colors.White);
                        });
                    });
                    col.Item().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(equip =>
                    {
                        equip.Item().Text("Equipment").FontSize(18).SemiBold().FontColor(Colors.Purple.Lighten2);
                        equip.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(1);
                            });
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Head").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Eyes").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Neck").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Shoulders").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Chest").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Back").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Wrists").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Hands").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Ring (left)").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Ring (right)").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Waist").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Legs").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Feet").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Main Hand").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Off Hand").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("Ammunition").FontColor(Colors.White);
                            table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                        });
                    });

                    col.Item().PageBreak();

                    col.Item().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(inv =>
                    {
                        inv.Item().Text("Inventory").FontSize(18).SemiBold().FontColor(Colors.Purple.Lighten2);
                        inv.Item().PaddingTop(4).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            for (int i = 0; i < 18; i++)
                            {
                                table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                                table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                                table.Cell().Padding(4).BorderBottom(1).BorderColor(Colors.Grey.Darken1).Text("").FontColor(Colors.White);
                            }
                        });
                    });
                    col.Item().Border(1).BorderColor(Colors.Purple.Medium).Padding(8).Column(footer =>
                    {
                        footer.Item().Text($"Legends of the Lance - Character Sheet - Generated at {DateTime.Now:MMM dd, yyyy HH:mm}").FontSize(12).SemiBold().FontColor(Colors.Purple.Lighten2);
                        footer.Item().Text($" UserName: {character.Username ?? "Unknown"}").FontColor(Colors.Grey.Lighten1);
                    });
                });
            });
        });
        return document.GeneratePdf();
    }
}





