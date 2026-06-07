using NoteNest.ViewModels;
using Xunit;

namespace NoteNest.Tests;

public class TaskBoardViewModelTests
{
    [Fact]
    public void MoveTaskAndBuildModelPreserveGroupOwnership()
    {
        var board = new TaskBoardViewModel();
        var changeCount = 0;
        board.Changed += (_, _) => changeCount++;
        var task = board.AddTask("today", "Task")!;

        Assert.NotNull(board.MoveTask(task, "backlog"));
        var model = board.BuildModel();
        Assert.Empty(model.Today);
        Assert.Equal("Task", Assert.Single(model.Backlog).Title);
        Assert.Equal(2, changeCount);
    }
}
