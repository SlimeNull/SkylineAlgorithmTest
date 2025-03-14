# SkylineAlgorithmTest

基于天空线(可能?)的二维打包算法, 和正常的二维打包不同的是, 此打包过程并未指定 "包" 的大小, 而是无限的控件, 矩形尽可能的紧凑布局, 保持近似方形, 占用更小的空间.

![](/Assets/preview.png)

## 如何使用

下载当前仓库中的 RectSpace.cs 文件, 它是布局算法的核心, 然后放到你的项目中. 初始化 RectSpace 对象实例, 并调用 Layout 方法:

```cs
bool Layout(int width, int height, out int x, out int y);
```

通过 RectSpace 的 Width 和 Height 属性可以获取当前已完成布局内容所占空间的外接矩形宽高. 通过 Rectangles 属性可以访问已经完成布局的所有矩形.