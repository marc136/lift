using System;
using System.Windows;

namespace Lift
{
    class DragDropHelper
    {
        public bool DraggingHasStarted { get { return StartPosition != null; } }

        public Point StartPosition { get; private set; }

        public void StartDraggingAtPoint(Point point)
        {
            StartPosition = point;
        }

        public bool MinimalDragDistanceWasExceeded(Point point)
        {
            if (!DraggingHasStarted) return false;
            Vector diff = StartPosition - point;

            return (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance);
        }
    }
}
