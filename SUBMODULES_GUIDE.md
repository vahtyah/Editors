# Hướng dẫn chi tiết sử dụng Git Submodules

## Packages

| Package | GitHub URL |
|---------|-----------|
| core | https://github.com/vahtyah/com.vahtyah.core |
| inspector | https://github.com/vahtyah/com.vahtyah.inspector |
| level-editor | https://github.com/vahtyah/com.vahtyah.level-editor |

---

## 1. Clone Project mới

**Command:**
```bash
# Clone với submodules (recommended)
git clone --recursive https://github.com/vahtyah/Editors.git

# Hoặc clone trước, init submodules sau
git clone https://github.com/vahtyah/Editors.git
cd Editors
git submodule update --init --recursive
```

**Fork:**
1. `File` → `Clone...`
2. Nhập URL: `https://github.com/vahtyah/Editors.git`
3. ✅ Check `Recursive` option
4. Click `Clone`

---

## 2. Pull updates từ remote

**Command:**
```bash
# Pull main repo
git pull

# Pull submodules updates
git submodule update --init --recursive
```

**Fork:**
1. Click `Pull` trên toolbar
2. Sau đó: `Repository` → `Submodules` → `Update All Submodules`

---

## 3. Chỉnh sửa code trong Package (Submodule)

### Bước 1: Checkout branch trong submodule

**Command:**
```bash
cd Assets/Packages/com.vahtyah.core
git checkout main
```

**Fork:**
1. Mở submodule: Double-click vào submodule trong sidebar (hoặc chuột phải → `Open Submodule`)
2. Checkout branch `main` (vì submodule mặc định ở detached HEAD)

### Bước 2: Chỉnh sửa, commit, push trong submodule

**Command:**
```bash
# Đang ở trong folder submodule
git add -A
git commit -m "Add new feature"
git push
```

**Fork:**
1. Trong cửa sổ submodule: Stage changes → Commit → Push

### Bước 3: Update reference trong main repo

**Command:**
```bash
# Quay về main repo
cd ../../..

# Add submodule reference change
git add Assets/Packages/com.vahtyah.core
git commit -m "Update core submodule"
git push
```

**Fork:**
1. Quay về main repo (click tên repo trong tabs)
2. Bạn sẽ thấy submodule có changes (hiện commit hash mới)
3. Stage → Commit → Push

---

## 4. Pull updates của một package cụ thể

**Command:**
```bash
# Pull latest từ remote của submodule
cd Assets/Packages/com.vahtyah.inspector
git pull origin main

# Hoặc từ main repo
git submodule update --remote Assets/Packages/com.vahtyah.inspector
```

**Fork:**
1. Mở submodule
2. Click `Pull`

---

## 5. Pull tất cả submodules về latest

**Command:**
```bash
git submodule update --remote --merge
```

**Fork:**
1. `Repository` → `Submodules` → `Update All Submodules`
2. Hoặc chuột phải submodule → `Update Submodule`

---

## 6. Xem trạng thái submodules

**Command:**
```bash
git submodule status
```

Output:
```
 841b9e5 Assets/Packages/com.vahtyah.core (heads/main)
 76ea091 Assets/Packages/com.vahtyah.inspector (heads/main)
 5c13a10 Assets/Packages/com.vahtyah.level-editor (heads/main)
```

**Fork:**
- Sidebar hiển thị tất cả submodules với trạng thái

---

## 7. Workflow thường ngày

```
┌─────────────────────────────────────────────────────────────┐
│  EDIT PACKAGE                                               │
├─────────────────────────────────────────────────────────────┤
│  1. cd Assets/Packages/com.vahtyah.core                     │
│  2. git checkout main  (nếu chưa)                           │
│  3. [Sửa code]                                              │
│  4. git add -A && git commit -m "message"                   │
│  5. git push                                                │
│  6. cd ../../..                                             │
│  7. git add . && git commit -m "Update submodule"           │
│  8. git push                                                │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  SYNC TỪ MÁY KHÁC                                           │
├─────────────────────────────────────────────────────────────┤
│  1. git pull                                                │
│  2. git submodule update --init --recursive                 │
└─────────────────────────────────────────────────────────────┘
```

---

## 8. Lỗi thường gặp

### "HEAD detached" trong submodule
```bash
cd Assets/Packages/com.vahtyah.core
git checkout main
```

### Submodule rỗng sau khi clone
```bash
git submodule update --init --recursive
```

### Conflict submodule reference
```bash
# Accept theirs (remote version)
git checkout --theirs Assets/Packages/com.vahtyah.core
git add Assets/Packages/com.vahtyah.core

# Hoặc accept ours (local version)
git checkout --ours Assets/Packages/com.vahtyah.core
git add Assets/Packages/com.vahtyah.core
```

---

## 9. Cài package vào project Unity khác (không dùng submodule)

Thêm vào `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.vahtyah.core": "https://github.com/vahtyah/com.vahtyah.core.git",
    "com.vahtyah.inspector": "https://github.com/vahtyah/com.vahtyah.inspector.git",
    "com.vahtyah.level-editor": "https://github.com/vahtyah/com.vahtyah.level-editor.git"
  }
}
```

Hoặc qua Unity:
1. `Window` → `Package Manager`
2. `+` → `Add package from git URL...`
3. Paste: `https://github.com/vahtyah/com.vahtyah.core.git`

---

## 10. Tips cho Fork

| Thao tác | Cách làm trong Fork |
|----------|---------------------|
| Mở submodule | Double-click submodule trong sidebar |
| Quay về main repo | Click tab repo chính |
| Update all submodules | `Repository` → `Submodules` → `Update All` |
| Xem submodule changes | Submodule hiện icon modified nếu có thay đổi |
| Add submodule mới | `Repository` → `Submodules` → `Add Submodule...` |
