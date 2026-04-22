using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZooWorld.Infrastructure.Data
{
    // Parses animal stats from CSV. Header row is required.
    // Columns: Id,Speed,Damage,Hp,Type,Subtype,JumpInterval,JumpDistance[,MovementType]
    //  - Type: 0 = Prey, 1 = Predator
    //  - Subtype: string value from AnimalSubtype enum
    //  - MovementType: optional; falls back to Jump when JumpInterval > 0, otherwise Linear.
    public class CsvAnimalDataLoader
    {
        private readonly TextAsset _textAsset;

        public CsvAnimalDataLoader(TextAsset textAsset)
        {
            _textAsset = textAsset;
        }

        public UniTask<IReadOnlyList<AnimalDataRow>> LoadAsync()
        {
            if (_textAsset == null)
            {
                Debug.LogError("[CsvAnimalDataLoader] CSV TextAsset is not assigned.");
                return UniTask.FromResult<IReadOnlyList<AnimalDataRow>>(Array.Empty<AnimalDataRow>());
            }

            string csv = _textAsset.text;
            if (string.IsNullOrWhiteSpace(csv))
            {
                Debug.LogError("[CsvAnimalDataLoader] CSV content is empty.");
                return UniTask.FromResult<IReadOnlyList<AnimalDataRow>>(Array.Empty<AnimalDataRow>());
            }

            IReadOnlyList<AnimalDataRow> rows = Parse(csv);
            return UniTask.FromResult(rows);
        }

        private static List<AnimalDataRow> Parse(string csv)
        {
            List<AnimalDataRow> result = new();
            string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return result;

            string[] header = SplitLine(lines[0]);
            Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < header.Length; i++)
                columns[header[i].Trim()] = i;

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.TrimStart().StartsWith("#")) continue;

                string[] parts = SplitLine(line);

                try
                {
                    int id = ParseInt(parts, columns, "Id");
                    float speed = ParseFloat(parts, columns, "Speed");
                    float damage = ParseFloat(parts, columns, "Damage");
                    float hp = ParseFloat(parts, columns, "Hp");
                    int typeInt = ParseInt(parts, columns, "Type");
                    AnimalRole role = typeInt == 1 ? AnimalRole.Predator : AnimalRole.Prey;
                    string subtypeRaw = ParseString(parts, columns, "Subtype");
                    if (!Enum.TryParse(subtypeRaw, true, out AnimalSubtype subtype))
                        subtype = AnimalSubtype.Unknown;
                    float jumpInterval = TryParseFloat(parts, columns, "JumpInterval", 0f);
                    float jumpDistance = TryParseFloat(parts, columns, "JumpDistance", 0f);
                    MovementType movementType = ParseMovementType(parts, columns, jumpInterval);

                    result.Add(new AnimalDataRow(id, speed, damage, hp, role, subtype, jumpInterval, jumpDistance, movementType));
                }
                catch (Exception e)
                {
                    Debug.LogError($"[CsvAnimalDataLoader] Failed to parse line {i}: '{line}'. {e.Message}");
                }
            }

            return result;
        }

        private static string[] SplitLine(string line)
        {
            string[] raw = line.Split(',');
            for (int i = 0; i < raw.Length; i++)
                raw[i] = raw[i].Trim();
            return raw;
        }

        private static int ParseInt(string[] parts, Dictionary<string, int> columns, string name)
        {
            string v = ParseString(parts, columns, name);
            return int.Parse(v, CultureInfo.InvariantCulture);
        }

        private static float ParseFloat(string[] parts, Dictionary<string, int> columns, string name)
        {
            string v = ParseString(parts, columns, name);
            return float.Parse(v, CultureInfo.InvariantCulture);
        }

        private static float TryParseFloat(string[] parts, Dictionary<string, int> columns, string name, float fallback)
        {
            if (!columns.TryGetValue(name, out int idx) || idx >= parts.Length) return fallback;
            string v = parts[idx];
            return float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : fallback;
        }

        private static MovementType ParseMovementType(string[] parts, Dictionary<string, int> columns, float jumpInterval)
        {
            if (columns.TryGetValue("MovementType", out int idx) && idx < parts.Length)
            {
                string raw = parts[idx];
                if (!string.IsNullOrWhiteSpace(raw)
                    && Enum.TryParse(raw, true, out MovementType parsed)
                    && parsed != MovementType.Unknown)
                    return parsed;
            }

            return jumpInterval > 0f ? MovementType.Jump : MovementType.Linear;
        }

        private static string ParseString(string[] parts, Dictionary<string, int> columns, string name)
        {
            if (!columns.TryGetValue(name, out int idx))
                throw new Exception($"Column '{name}' not found in CSV header.");
            if (idx >= parts.Length)
                throw new Exception($"Row has fewer columns than expected (missing '{name}').");
            return parts[idx];
        }
    }
}
