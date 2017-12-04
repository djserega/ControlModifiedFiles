using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ControlModifiedFiles
{
    internal class SaveLoadConfig
    {
        private string _pathConfig = Environment.CurrentDirectory + "\\datarow.json";

        internal bool SaveConfig(List<FileInfo> list)
        {
            try
            {
                var serializer = new JavaScriptSerializer();

                new DirFile().SaveFile(
                    _pathConfig,
                    serializer.Serialize(
                        serializer.ConvertToType<List<FileInfo>>(list)));

                Dialog.ShowMessage("Настройки успешно сохранены.");
                return true;
            }
            catch (Exception)
            {
                Dialog.ShowMessage("Ошибка сохранения настроек.");
                return false;
            }
        }

        internal List<FileInfo> LoadConfig()
        {
            try
            {
                List<FileInfo> list = new JavaScriptSerializer().Deserialize<List<FileInfo>>(
                    new DirFile().LoadFile(_pathConfig));
                Dialog.ShowMessage("Настройки успшешно восстановлены.");
                return list;
            }
            catch (Exception)
            {
                Dialog.ShowMessage("Ошибка восстановления настроек.");
                return null;
            }
        }
    }
}
