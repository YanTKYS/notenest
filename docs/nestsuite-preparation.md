# NestSuite対応準備メモ

## 目的
NoteNestを将来的にNestSuiteへ統合できるよう、単体アプリの外枠とNoteNest固有の作業領域を分離して考える。

## AppShell側の責務
- MainWindow
- メニュー
- StartDialog
- RecentFiles
- Open / Save / SaveAs
- Exit confirmation
- Window settings

## Workspace側の責務
- NoteNestWorkspaceView
- NoteNestWorkspaceViewModel
- NoteWorkspaceViewModel
- TaskBoardViewModel
- MarkerPanelViewModel
- EditorStateViewModel
- ProjectSessionViewModel
- WorkspaceChangeCoordinator
- Project data editing
- Marker extraction
- Note/task operations

## NestSuite移行時に再利用したいもの
- Workspace系ViewModel
- Project model
- Project file service
- Marker / Task / Note services
- Markdown export core

## NestSuite移行時に置き換えるもの
- MainWindow
- App-level menu
- Recent files
- Startup flow
- File open/save shell

## 当面の方針
- NoteNest単体版は維持する
- ただし、MainWindowは将来的なAppShellと見なす
- Workspace部分をNestSuiteへ移せるよう、AppShell依存を増やさない
- すぐに大規模なView切り出しは行わない
