using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedImport
{
    public static class Error
    {
        public static void FileReadError(Exception ex) {
            MessageBox.Show($"Error reading file: {ex.GetType()}", "Error opening file",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void TilemapExportError(Exception ex) {
            MessageBox.Show($"Error exporting tilemap: {ex.GetType()}", "Error exporting tilemap",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void TilesetExportError(Exception ex)
        {
            MessageBox.Show($"Error exporting tileset: {ex.GetType()}", "Error exporting tileset",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ProjectExportError(Exception ex)
        {
            MessageBox.Show($"Error exporting project: {ex.GetType()}", "Error exporting project",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void NoSaveLocationError()
        {
            MessageBox.Show($"No save location selected. Please select a valid location", "No save location",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void NoFileSelectedError()
        {
            MessageBox.Show($"No file selected. Please select a valid file", "No file selected",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void DeserializeError(Exception ex)
        {
            MessageBox.Show($"Error deserializing file: {ex.GetType()}", "Error deserializing file",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void SerializeError(Exception ex) {
            MessageBox.Show($"Error serializing file: {ex.GetType()}", "Error serializing file",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void NoTrackSelectedError()
        {
            MessageBox.Show("No Track Selected, please select a track", "Track selection warning", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void ZoneCountError()
        {
            MessageBox.Show($"Tracks cannot have more than 255 zones", "Error reading zones",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void TargetCountError()
        {
           MessageBox.Show($"Each target group must have the same number of targets as there are zones", "Error reading targets",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void InvalidZoneError()
        {
            MessageBox.Show($"Invalid triangle zone in AI zones", "Error reading zones",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ImageFormatError()
        {
            MessageBox.Show("Only 8 bpp paletted images are supported", "Image type error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
