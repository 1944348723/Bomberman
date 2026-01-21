[TOC]

# 地图系统
采用TileMap绘制地图
**关键配置点：**
* `Pixels per unit`设置成和图片像素大小相同，`Grid`的`Cell Size`设置为1，`Sprite`的`Pivot`设置为(0.5, 0.5)，`Tilemap`的`Anchor Point`设置为(0.5, 0.5)，这样图片就完全和`Grid`的格子重合
* 将`Grid`所在`GameObect`的`Transform`的位置设置为(-0.5, -0.5)，这样做能让`Grid`的(0, 0)处的格子中心和世界坐标的(0, 0)对齐，之后将图片的位置直接设置成(1, 1)的话就直接在`Grid`的(1, 1)格子中心，并且`Tilemap`的`WorldToCell()`检测也会更符合我们预期。`Grid`的`CellToWorld()`时，是将格子左下角的坐标进行转换，也就是格子是以左下角为主的，检测就是判断是否在左下角到右上角的范围内。像这样设置后，(0.0, 0.0)到(1.0, 1.0)的范围内会被检测为(0,0)Cell，符合直觉，不会错位


# 对象池和回收
本项目中所有会重复创建销毁的对象都进行了池化

这些对象都有`AnimatedSpriteRenderer`、`AnimationAutoReleaser`组件，`AnimationAutoReleaser`组件是用来连接`AnimatedSpriteRenderer`和`PoolManager`的。

在挂了`AnimationAutoReleaser`组件并填写对象池名后，会在`AnimatedSpriteRenderer`动画播放完成后自动将此`GameObject`回收至填写的对象池中。

注意`AnimatedSpriteRenderer`中的`destroyOnFinished`选项和`AnimationAutoReleaser`不要同时使用，这是两种不同的工作流