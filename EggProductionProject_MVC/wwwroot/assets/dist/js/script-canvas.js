
const world = document.querySelector(".boops");
const { Engine, Render, Runner, World, Bodies, Mouse, MouseConstraint, Body } = Matter;

let centerImage; // 將圖片設為全局變量

// 先加載圖片並獲取其寬高
function loadImageDimensions(src) {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.src = src;
    img.onload = () => resolve({ width: img.width, height: img.height });
    img.onerror = reject;
  });
}

// 自動檢測圖片大小並創建中心圖片
async function createCenterImage() {
    const imgSrc = "../assets/brand/page/index/GOOD EGG.svg"; // 替換成你的圖片路徑
  try {
    const { width, height } = await loadImageDimensions(imgSrc);

    // 設定最大和最小的縮放比例
    const maxScaleFactor = 0.5;
    const minScaleFactor = 0.30;

    // 根據視窗寬度動態調整縮放比例
    let scaleFactor = window.innerWidth < 768 ? minScaleFactor : maxScaleFactor;

    // 縮放比例控制在minScaleFactor和maxScaleFactor之間
    scaleFactor = Math.max(minScaleFactor, Math.min(scaleFactor, maxScaleFactor));

    const scaledWidth = width * scaleFactor;
    const scaledHeight = height * scaleFactor;

    // 計算畫布中心位置，固定高度
    const canvasCenterX = window.innerWidth / 2;
    const canvasCenterY = 600 / 2 - 80; // 固定高度為600，圖片向上偏移80像素

    // 創建圖片，設置為全局變量
    centerImage = Bodies.rectangle(canvasCenterX, canvasCenterY, scaledWidth, scaledHeight, {
      isStatic: true,
      render: {
        sprite: {
          texture: imgSrc,
          xScale: scaleFactor,
          yScale: scaleFactor
        }
      }
    });

    World.add(engine.world, [centerImage]);
  } catch (error) {
    console.error("Failed to load image:", error);
  }
}


// 調整中心圖片位置
function updateCenterImagePosition() {
  if (centerImage) {
    const canvasCenterX = window.innerWidth / 2;
    const canvasCenterY = 600 / 2 - 150; // 固定高度時的中心
    Body.setPosition(centerImage, { x: canvasCenterX, y: canvasCenterY });
  }
}

function createBall() {
  const ball = Bodies.circle(Math.round(Math.random() * window.innerWidth), -30, 25, {
    angle: Math.PI * (Math.random() * 2 - 1),
    friction: 0.001,
    frictionAir: 0.01,
    restitution: 0.8,
    render: {
      sprite: {
            texture: "../assets/brand/page/index/eggs6-removebg-preview.svg",
        xScale: 0.1, // 縮小一半
        yScale: 0.1
      }
    }
  });

  setTimeout(() => {
    World.remove(engine.world, ball);
  }, 30000);

  return ball;
}

function resizeCanvas() {
    world.width = window.innerWidth;
    world.height = Math.min(window.innerHeight, 600); // 設置畫布高度為 600，或視口高度的最小值
}

// 初次設置 canvas 大小
resizeCanvas();

const engine = Engine.create();
const runner = Runner.create();
const render = Render.create({
  canvas: world,
  engine: engine,
  options: {
    width: window.innerWidth, // 設置寬度為瀏覽器窗口的寬度
    height: Math.min(window.innerHeight, 600), // 設置高度為窗口高度或 600 的最小值
    background: "transparent",
    wireframes: false
  }
});

// 當窗口大小改變時，重新調整畫布寬度
window.addEventListener('resize', () => {
  render.canvas.width = window.innerWidth;
  render.canvas.height = Math.min(window.innerHeight, 600); // 限制畫布的最大高度為 600
  render.options.width = window.innerWidth;

  // 調整邊界和圖片位置
  Body.setPosition(ground, { x: window.innerWidth / 2, y: 600 });
  Body.setPosition(rightWall, { x: window.innerWidth, y: 300 }); // 修改牆的Y坐標
  updateCenterImagePosition();
});

// 邊界設置
const boundaryOptions = {
  isStatic: true,
  render: {
    fillStyle: "transparent",
    strokeStyle: "transparent"
  }
};

// 更新左右牆的寬度和高度
const ground = Bodies.rectangle(window.innerWidth / 2, 600, window.innerWidth, 4, boundaryOptions);
const leftWall = Bodies.rectangle(0, 300, 4, 600, boundaryOptions); // 高度設置為600px
const rightWall = Bodies.rectangle(window.innerWidth, 300, 4, 600, boundaryOptions); // 高度設置為600px

Render.run(render);
Runner.run(runner, engine);

// 添加邊界
World.add(engine.world, [ground, leftWall, rightWall]);

// 創建中心圖片，並自動偵測尺寸
createCenterImage();

// 當滾輪滾動時，阻止頁面上下滑動
world.addEventListener('wheel', (e) => {
  e.preventDefault();
  window.scrollBy(0, e.deltaY);
});

// 添加拖拽功能
const mouse = Mouse.create(render.canvas);
const mouseConstraint = MouseConstraint.create(engine, {
  mouse: mouse,
  constraint: {
    stiffness: 0.2,
    render: {
      visible: false
    }
  }
});

World.add(engine.world, mouseConstraint);

const handleClick = () => {
  const ball2 = createBall();
  World.add(engine.world, [ball2]);
};

const button = document.querySelector("#boop");
button.addEventListener("click", handleClick);
