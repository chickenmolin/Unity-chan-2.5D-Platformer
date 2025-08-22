# Camera System Scripts

Bộ script quản lý camera cho Unity với Cinemachine integration.

## Scripts

### 1. CameraSwitcher.cs
**Chức năng:** Chuyển đổi giữa các Virtual Camera của Cinemachine

**Cách sử dụng:**
```csharp
// Chuyển đến camera tiếp theo
CameraSwitcher.Instance.SwitchToNextCamera();

// Chuyển đến camera theo index
CameraSwitcher.Instance.SwitchToCamera(2);

// Chuyển đến camera theo tên
CameraSwitcher.Instance.SwitchToCamera("PlayerCamera");
```

**Input mặc định:**
- `Tab`: Camera tiếp theo
- `Shift`: Camera trước đó  
- `1-9`: Chuyển đến camera theo số

### 2. CameraFollowPlayer.cs
**Chức năng:** Camera theo dõi player một cách mượt mà

**Features:**
- Tự động tìm player
- Offset và damping settings
- Boundaries giới hạn
- Prediction movement
- Obstacle avoidance

**Thiết lập:**
- Gắn vào Virtual Camera
- Kéo Player vào `Player Target` (hoặc để auto find)
- Điều chỉnh `Follow Offset` và `Damping`

### 3. CameraZone.cs
**Chức năng:** Tự động chuyển camera khi player vào/ra vùng

**Cách sử dụng:**
1. Tạo GameObject với Collider (Trigger)
2. Gắn script CameraZone
3. Thiết lập Target Camera hoặc Camera Index
4. Điều chỉnh settings

**Settings quan trọng:**
- `Target Camera`: Camera sẽ chuyển đến
- `Activate On Enter`: Bật camera khi vào zone
- `Deactivate On Exit`: Tắt camera khi ra zone
- `Activation Delay`: Delay trước khi chuyển

### 4. CameraShake.cs
**Chức năng:** Tạo hiệu ứng rung camera

**Cách sử dụng:**
```csharp
// Shake cơ bản
CameraShake.Instance.Shake(1f, 0.5f);

// Shake tại vị trí (tính khoảng cách)
CameraShake.Instance.ShakeAtPosition(explosionPos, 2f, 1f);

// Shake nhanh (bắn súng)
CameraShake.Instance.ImpactShake(0.5f);

// Shake dài (động đất)
CameraShake.Instance.ContinuousShake(0.3f, 5f);
```

**Integration với Cinemachine:**
- Tự động setup Perlin Noise component
- Sử dụng Noise Profiles có sẵn
- Hỗ trợ frequency-based shake

### 5. CameraTransition.cs
**Chức năng:** Hiệu ứng chuyển cảnh mượt mà

**Loại transitions:**
- `Instant`: Chuyển ngay
- `Fade`: Fade đen
- `Slide`: Trượt camera
- `Zoom`: Zoom in/out
- `CrossDissolve`: Blend mượt

**Cách sử dụng:**
```csharp
// Transition với hiệu ứng mặc định
CameraTransition.Instance.TransitionToCamera(fromCam, toCam);

// Transition với settings custom
CameraTransition.Instance.TransitionToCamera(fromCam, toCam, 
    TransitionType.Fade, 2f);

// Transition với CameraSwitcher
CameraTransition.Instance.TransitionToCamera(2, TransitionType.Slide);
```

### 6. CameraConfinerManager.cs
**Chức năng:** Quản lý Cinemachine Confiner cho từng Virtual Camera

**Features:**
- Tự động tạo confiner cho mỗi camera
- Quản lý bounds riêng biệt
- Tích hợp với CameraSwitcher
- Hỗ trợ Tilemap bounds

**Cách sử dụng:**
```csharp
// Thêm camera với confiner
CameraConfinerManager.Instance.AddCamera(virtualCamera);

// Thay đổi bounds
CameraConfinerManager.Instance.ChangeCameraBounds(camera, newBounds);

// Resize bounds
CameraConfinerManager.Instance.ResizeCameraBounds(camera, new Vector2(30, 20));
```

### 7. CameraConfinerSetup.cs
**Chức năng:** Helper script để nhanh chóng setup confiner cho từng camera

**Features:**
- Gắn trực tiếp vào Virtual Camera
- Auto setup confiner khi start
- Tạo bounds từ Tilemap
- Preview bounds trong Scene view

**Thiết lập:**
1. Gắn script vào Virtual Camera GameObject
2. Điều chỉnh `Bounds Size` và `Bounds Offset`
3. Script sẽ tự động tạo confiner khi start

## Setup hướng dẫn

### Bước 1: Cài đặt Cinemachine
1. Window → Package Manager
2. Tìm "Cinemachine" và install

### Bước 2: Setup CameraSwitcher
1. Tạo Empty GameObject → đặt tên "Camera Manager"
2. Gắn script `CameraSwitcher`
3. Kéo các Virtual Camera vào list `Virtual Cameras`

### Bước 3: Setup Virtual Cameras
1. Cinemachine → Create Virtual Camera
2. Gắn script `CameraFollowPlayer` nếu cần follow player
3. Điều chỉnh settings

### Bước 4: Setup Camera Zones (Optional)
1. Tạo GameObject với Collider (Box/Sphere)
2. Check "Is Trigger"
3. Gắn script `CameraZone`
4. Thiết lập target camera

### Bước 5: Setup Confiner (Optional)
1. Tạo GameObject → đặt tên "Camera Confiner Manager"
2. Gắn script `CameraConfinerManager`
3. Hoặc gắn `CameraConfinerSetup` trực tiếp vào từng Virtual Camera

### Bước 6: Setup Effects (Optional)
1. Tạo GameObject → đặt tên "Camera Effects"
2. Gắn `CameraShake` và `CameraTransition`

## Events và Callbacks

Tất cả scripts đều có events để integration:

```csharp
// CameraSwitcher
CameraSwitcher.Instance.OnCameraChanged += (index) => {
    Debug.Log($"Chuyển đến camera {index}");
};

// CameraShake  
CameraShake.Instance.OnShakeStarted += (intensity, duration) => {
    // Xử lý khi bắt đầu shake
};

// CameraTransition
CameraTransition.Instance.OnTransitionCompleted += (from, to) => {
    // Xử lý khi hoàn thành transition
};
```

## Tips và Notes

1. **Performance:** Chỉ một CameraSwitcher Instance trong scene
2. **Cinemachine Brain:** Main Camera tự động có Cinemachine Brain
3. **Layer Settings:** Đảm bảo Player có đúng tag hoặc layer
4. **Transition:** Dùng CameraTransition cho smooth effects
5. **Mobile:** Camera shake có thể ảnh hưởng performance trên mobile

## Troubleshooting

**Camera không chuyển:**
- Kiểm tra Priority của Virtual Cameras
- Đảm bảo CameraSwitcher.Instance không null

**Player không được detect trong CameraZone:**
- Kiểm tra Player Tag
- Đảm bảo Collider là Trigger
- Kiểm tra Layer Mask settings

**Shake không hoạt động:**
- Đảm bảo có CameraShake.Instance
- Kiểm tra target camera/virtual camera
- Verify Cinemachine Noise component

**Confiner không hoạt động:**
- Đảm bảo Collider2D là Trigger
- Kiểm tra CinemachineConfiner2D component
- Verify bounds collider có đúng layer

**Camera không bị giới hạn bởi bounds:**
- Kiểm tra Bounding Shape 2D trong Confiner
- Đảm bảo bounds đủ lớn cho camera
- Verify Damping settings