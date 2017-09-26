using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CreateMergedSymbolicLinkFolder
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("使い方: CreateMergedSymbolicLinkFolder.exe 階層 リンク作成先フォルダ リンク作成元フォルダ1 [リンク作成元フォルダ2...]");
                return 2;
            }

            if (!int.TryParse(args[0], out var targetHierarchy) || targetHierarchy < 1)
            {
                Console.Error.WriteLine("階層には1以上の整数を指定してください");
                return 2;
            }

            var dstDir = args[1];
            if (!Directory.Exists(dstDir))
            {
                Console.Error.WriteLine("リンク作成先フォルダ {0} が存在していません", dstDir);
                return 2;
            }

            var srcDirs = args.Skip(2).ToArray();
            var notExists = srcDirs.FirstOrDefault(x => !Directory.Exists(x));
            if (notExists != null)
            {
                Console.Error.WriteLine("リンク作成元フォルダ {0} が存在していません", notExists);
                return 2;
            }

            var addedPaths = new HashSet<string>();
            dstDir = Path.GetFullPath(dstDir);
            foreach (var srcDir in srcDirs.Select(x => Path.GetFullPath(x)))
            {
                void doRecurse(string relativeDir, int hierarchy)
                {
                    var subDirs = new DirectoryInfo(Path.Combine(srcDir, relativeDir)).GetDirectories();
                    foreach (var subDir in subDirs)
                    {
                        var newRelativeDir = Path.Combine(relativeDir, subDir.Name);
                        if (hierarchy >= targetHierarchy)
                        {
                            // 対象の改装の場合はシンボリックリンクを張る

                            // シンボリックリンクを張った場合はスキップ
                            var linkPath = Path.Combine(dstDir, newRelativeDir);
                            if (addedPaths.Contains(linkPath)) continue;

                            var targetPath = Path.Combine(srcDir, newRelativeDir);
                            // 既にシンボリックが存在していた場合はターゲットが正しいかをチェック
                            if (Directory.Exists(linkPath))
                            {
                                if (SymbolicLink.Exists(linkPath) && SymbolicLink.GetTarget(linkPath) == targetPath)
                                {
                                    // 正しければ張ったものとしてスキップ

                                    addedPaths.Add(linkPath);
                                    continue;
                                }
                                else
                                {
                                    // シンボリックリンクが無効だったりターゲットが違う場合は削除

                                    Directory.Delete(linkPath);
                                }
                            }

                            SymbolicLink.CreateDirectoryLink(linkPath, targetPath);
                            addedPaths.Add(linkPath);
                        }
                        else
                        {
                            // 対象の階層になるまでディレクトリを生成

                            Directory.CreateDirectory(Path.Combine(dstDir, newRelativeDir));
                            doRecurse(newRelativeDir, hierarchy + 1);
                        }
                    }
                }
                doRecurse("", 1);
            }

            return 0;
        }
    }


}
