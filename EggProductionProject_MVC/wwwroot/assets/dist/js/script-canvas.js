const world = document.querySelector(".boops");
const { Engine, Render, Runner, World, Bodies, Mouse, MouseConstraint, Body } = Matter;

let centerImage;

// 加載圖片並獲取其寬高
function loadImageDimensions(src) {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.src = src;
        img.onload = () => resolve({ width: img.width, height: img.height });
        img.onerror = reject;
    });
}

async function createCenterImage() {
    const imgSrc = "../assets/brand/page/index/GOOD EGG.svg";
    try {
        const { width, height } = await loadImageDimensions(imgSrc);
        const maxScaleFactor = 0.5;
        const minScaleFactor = 0.30;
        let scaleFactor = window.innerWidth < 768 ? minScaleFactor : maxScaleFactor;
        scaleFactor = Math.max(minScaleFactor, Math.min(scaleFactor, maxScaleFactor));

        const scaledWidth = width * scaleFactor;
        const scaledHeight = height * scaleFactor;

        const canvasCenterX = window.innerWidth / 2;
        const canvasCenterY = window.innerHeight / 2 - 80;

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

function updateCenterImagePosition() {
    if (centerImage) {
        const canvasCenterX = window.innerWidth / 2;
        const canvasCenterY = window.innerHeight / 2;
        Body.setPosition(centerImage, { x: canvasCenterX, y: canvasCenterY });
    }
}

function resizeCanvas() {
    world.width = window.innerWidth;
    world.height = window.innerHeight;
    render.canvas.width = window.innerWidth;
    render.canvas.height = window.innerHeight;
}

const engine = Engine.create();
const runner = Runner.create();
const render = Render.create({
    canvas: world,
    engine: engine,
    options: {
        width: window.innerWidth,
        height: window.innerHeight,
        background: "transparent",
        wireframes: false
    }
});

// 初始設置畫布大小
resizeCanvas();

window.addEventListener('resize', () => {
    resizeCanvas();
    Body.setPosition(ground, { x: window.innerWidth / 2, y: window.innerHeight });
    Body.setPosition(rightWall, { x: window.innerWidth, y: window.innerHeight / 2 });
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

const ground = Bodies.rectangle(window.innerWidth / 2, window.innerHeight, window.innerWidth, 4, boundaryOptions);
const leftWall = Bodies.rectangle(0, window.innerHeight / 2, 4, window.innerHeight, boundaryOptions);
const rightWall = Bodies.rectangle(window.innerWidth, window.innerHeight / 2, 4, window.innerHeight, boundaryOptions);

Render.run(render);
Runner.run(runner, engine);

World.add(engine.world, [ground, leftWall, rightWall]);
createCenterImage();

world.addEventListener('wheel', (e) => {
    e.preventDefault();
    window.scrollBy(0, e.deltaY);
});

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

function createBall() {
    const ball = Bodies.circle(Math.round(Math.random() * window.innerWidth), -30, 25, {
        angle: Math.PI * (Math.random() * 2 - 1),
        friction: 0.001,
        frictionAir: 0.01,
        restitution: 0.8,
        render: {
            sprite: {
                texture: "../assets/brand/page/index/eggs6-removebg-preview.svg",
                xScale: 0.1,
                yScale: 0.1
            }
        }
    });

    setTimeout(() => {
        World.remove(engine.world, ball);
    }, 30000);

    return ball;
}


World.add(engine.world, mouseConstraint);

const handleClick = () => {
    const ball2 = createBall();
    World.add(engine.world, [ball2]);
};

const button = document.querySelector("#boop");
button.addEventListener("click", handleClick);
