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
        private string _pathConfig = Environment.CurrentDirectory + "\\datarows.json";

        internal bool SaveConfig(List<FileSubscriber> list)
        {
            try
            {
                var serializer = new JavaScriptSerializer();

                new DirFile().SaveFile(
                    _pathConfig,
                    serializer.Serialize(
                        serializer.ConvertToType<List<FileSubscriber>>(list)));

                Dialog.ShowMessage("Настройки сохранены.");
                return true;
            }
            catch (Exception)
            {
                Dialog.ShowMessage("Ошибка сохранения настроек.");
                return false;
            }
        }

        internal List<FileSubscriber> LoadConfig()
        {
            try
            {
                List<FileSubscriber> list = new JavaScriptSerializer().Deserialize<List<FileSubscriber>>(
                    new DirFile().LoadFile(_pathConfig));
                Dialog.ShowMessage("Настройки восстановлены.");
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
