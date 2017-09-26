# CreateMergedSymbolicLinkFolder
ハイレゾ音源とiTunes用音源を管理している環境で、DLNAで配信する際にハイレゾ音源を優先して配信するために、
配信用フォルダにハイレゾ音源とiTunes音源のシンボリックリンクを生成するツール。

## 使い方
```
> CreateMergedSymbolicLinkFolder 2 D:\Twonky\Music D:\HiResAudio D:\iTunesMedia\Music
```

最初の引数はディレクトリの階層を表す。
上の例ではアーティスト+アルバムの2階層なので2としている。
