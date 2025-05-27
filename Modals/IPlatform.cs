using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doodle_Jump.Modals
{
    public interface IPlatform
    {
        PointF Position { get; set; }
        SizeF Size { get; }

        /// Вызывается, когда игрок «приземлился» на платформу.
        /// Должна вернуть true, если платформа ещё существует
        /// (false — значит разрушилась и её надо удалить).
        bool OnLand(Player player);

        /// Отрисовать себя
        void Draw(Graphics g);
    }
}
