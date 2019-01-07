using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;


namespace Sample.iOS
{
    public class ScannerOverlayView : UIView
    {
        UILabel footerLabel;
        UIImageView animateLine;
        UIButton backImage;
        private UIColor _lineColor;

        /** 扫描器边框宽度 */
        private const float Scanner_BorderWidth = 1;
        /** 扫描器棱角宽度 */
        private const float Scanner_CornerWidth = 3;
        /** 扫描器棱角长度 */
        private const float Scanner_CornerLength = 20;
        /** 扫描器线条高度 */
        private const float Scanner_LineHeight = 10;

        private nfloat Scanner_Width;
        private nfloat Scanner_X;
        private nfloat Scanner_Y;
        private CABasicAnimation scanNetAnimation;

        public Action OnCancel;
        public Action OnResume;
        public Action OnPause;

        public ScannerOverlayView() : base()
        {
            BackgroundColor = UIColor.Clear;
            Scanner_Width = UIScreen.MainScreen.Bounds.Size.Width * 0.75f;
            Scanner_X = (UIScreen.MainScreen.Bounds.Size.Width - Scanner_Width) / 2;
            Scanner_Y = (UIScreen.MainScreen.Bounds.Size.Height - Scanner_Width) / 2 - 40;

            _lineColor = UIColor.FromRGB(0, 120, 215);

            NSNotificationCenter.DefaultCenter.AddObserver(this, new ObjCRuntime.Selector("appwillResignActive:"), UIApplication.WillResignActiveNotification, null);
            NSNotificationCenter.DefaultCenter.AddObserver(this, new ObjCRuntime.Selector("appBecomeActive:"), UIApplication.DidBecomeActiveNotification, null);

            backImage = new UIButton()
            {
                Frame = new CGRect(10, 30, 42, 42),
            };
            backImage.AdjustsImageWhenHighlighted = false;
            backImage.SetImage(UIImage.FromFile("close.png"), UIControlState.Normal);
            backImage.TouchUpInside += (sender, e) =>
            {
                if (OnCancel != null)
                    OnCancel.Invoke();
            };
            this.AddSubview(backImage);

            animateLine = new UIImageView()
            {
                Frame = new CGRect(Scanner_X, Scanner_Y, Scanner_Width, Scanner_LineHeight),
                Image = UIImage.FromFile("scan_line.png")
            };
            this.AddSubview(animateLine);

            footerLabel = new UILabel()
            {
                Text = "将二维码放入框内, 即可自动扫描",
                Font = UIFont.SystemFontOfSize(13),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Lines = 0,
                BackgroundColor = UIColor.Clear,
                Frame = new CGRect(0, Scanner_Y + Scanner_Width, UIScreen.MainScreen.Bounds.Width, 50)
            };
            this.AddSubview(footerLabel);
        }

        #region App 状态监听处理
        [Export("appwillResignActive:")]
        private void AppwillResignActive(NSNotification note)
        {
            if (OnResume != null)
                OnResume.Invoke();
            PauseScannerLineAnimation();
        }

        [Export("appBecomeActive:")]
        private void AppBecomeActive(NSNotification note)
        {
            if (OnPause != null)
                OnPause.Invoke();
            AddScannerLineAnimation();
        }
        #endregion

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            AddScannerLineAnimation();
        }

        public void AddScannerLineAnimation()
        {
            animateLine.Layer.RemoveAllAnimations();

            scanNetAnimation = CABasicAnimation.FromKeyPath("transform");
            scanNetAnimation.To = NSValue.FromCATransform3D(CATransform3D.MakeTranslation(0, Scanner_Width - Scanner_LineHeight, 1));
            scanNetAnimation.Duration = 4;
            scanNetAnimation.RepeatCount = float.MaxValue;
            animateLine.Layer.AddAnimation(scanNetAnimation, "ScannerLineAnmationKey");
            animateLine.Layer.Speed = 1.0f;
        }

        public void PauseScannerLineAnimation()
        {
            // 取出当前时间，转成动画暂停的时间
            double pauseTime = animateLine.Layer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null);
            // 设置动画的时间偏移量，指定时间偏移量的目的是让动画定格在该时间点的位置
            animateLine.Layer.TimeOffset = pauseTime;
            // 将动画的运行速度设置为0， 默认的运行速度是1.0
            animateLine.Layer.Speed = 0;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            // 半透明区域
            UIColor.FromWhiteAlpha(0, 0.7f).SetFill();
            UIGraphics.RectFill(rect);

            //// 透明区域
            CGRect scanner_rect = new CGRect(Scanner_X, Scanner_Y, Scanner_Width, Scanner_Width);
            UIColor.Clear.SetFill();
            UIGraphics.RectFill(scanner_rect);

            // 边框
            UIBezierPath borderPath = UIBezierPath.FromRect(new CGRect(Scanner_X, Scanner_Y, Scanner_Width, Scanner_Width));
            borderPath.LineCapStyle = CGLineCap.Round;
            borderPath.LineWidth = Scanner_BorderWidth;
            UIColor.White.SetColor();
            borderPath.Stroke();

            for (int index = 0; index < 4; ++index)
            {
                UIBezierPath tempPath = UIBezierPath.Create();
                tempPath.LineWidth = Scanner_CornerWidth;
                _lineColor.SetColor();

                switch (index)
                {
                    // 左上角棱角
                    case 0:
                        {
                            tempPath.MoveTo(new CGPoint(Scanner_X + Scanner_CornerLength, Scanner_Y));
                            tempPath.AddLineTo(new CGPoint(Scanner_X, Scanner_Y));
                            tempPath.AddLineTo(new CGPoint(Scanner_X, Scanner_Y + Scanner_CornerLength));
                        }
                        break;
                    // 右上角
                    case 1:
                        {
                            tempPath.MoveTo(new CGPoint(Scanner_X + Scanner_Width - Scanner_CornerLength, Scanner_Y));
                            tempPath.AddLineTo(new CGPoint(Scanner_X + Scanner_Width, Scanner_Y));
                            tempPath.AddLineTo(new CGPoint(Scanner_X + Scanner_Width, Scanner_Y + Scanner_CornerLength));
                        }
                        break;
                    // 左下角
                    case 2:
                        {
                            tempPath.MoveTo(new CGPoint(Scanner_X, Scanner_Y + Scanner_Width - Scanner_CornerLength));
                            tempPath.AddLineTo(new CGPoint(Scanner_X, Scanner_Y + Scanner_Width));
                            tempPath.AddLineTo(new CGPoint(Scanner_X + Scanner_CornerLength, Scanner_Y + Scanner_Width));
                        }
                        break;
                    // 右下角
                    case 3:
                        {
                            tempPath.MoveTo(new CGPoint(Scanner_X + Scanner_Width - Scanner_CornerLength, Scanner_Y + Scanner_Width));
                            tempPath.AddLineTo(new CGPoint(Scanner_X + Scanner_Width, Scanner_Y + Scanner_Width));
                            tempPath.AddLineTo(new CGPoint(Scanner_X + Scanner_Width, Scanner_Y + Scanner_Width - Scanner_CornerLength));
                        }
                        break;
                    default:
                        break;
                }
                tempPath.Stroke();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
            animateLine.Layer.RemoveAllAnimations();
            scanNetAnimation.Dispose();
            scanNetAnimation = null;
            footerLabel = null;
            animateLine = null;
        }

    }
}