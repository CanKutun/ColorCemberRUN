@echo off
setlocal EnableExtensions EnableDelayedExpansion
chcp 65001 >nul

REM ==== AYARLAR ====
set "REPO_DIR=C:\ColorCemberRUN"
set "MAIN_BRANCH=main"

title ColorCemberRun - Versiyon Yukselt + Guvenli Push (Lokal -> Uzak)

echo.
echo ===============================
echo  Projeye giriliyor...  (%REPO_DIR%)
echo ===============================
pushd "%REPO_DIR%" || (echo [HATA] Klasor bulunamadi: %REPO_DIR% & pause & exit /b)

REM ==== Git var mi? ====
git --version >nul 2>&1 || (echo [HATA] Git bulunamadi. Git kurulu mu? & pause & exit /b)

REM ==== Repo kurulumu / remote ====
if not exist ".git" (
  echo [INFO] Bu klasor Git reposu degil. Baslatiliyor...
  git init || (echo [HATA] git init basarisiz. & pause & exit /b)
)

REM ====== REMOTE KONTROL (yalniz URL fix) ======
set "CURURL="
for /f "delims=" %%u in ('git config --get remote.origin.url 2^>nul') do set "CURURL=%%u"
if defined CURURL goto REMOTE_OK
goto REMOTE_MISSING

:REMOTE_OK
echo [BILGI] Remote origin zaten tanimli: %CURURL%
goto REMOTE_DONE

:REMOTE_MISSING
echo [ONEMLI] Remote origin URL'ini gir (ornegin: https://github.com/CanKutun/ColorCemberRUN.git)
set "REMOTE_URL="
set /p REMOTE_URL="Remote URL: "
if not defined REMOTE_URL (echo [HATA] URL girilmedi. & pause & exit /b)
git remote add origin "%REMOTE_URL%" || (echo [HATA] remote ekleme basarisiz. & pause & exit /b)
goto REMOTE_DONE

:REMOTE_DONE
REM ====== /REMOTE KONTROL ======

REM ==== Branch kontrol ====
for /f "tokens=* usebackq" %%i in (`git rev-parse --abbrev-ref HEAD`) do set CURBR=%%i
if /i not "%CURBR%"=="%MAIN_BRANCH%" (
  echo [INFO] %CURBR% -> %MAIN_BRANCH% geciliyor/olusturuluyor...
  git checkout -B %MAIN_BRANCH% || (echo [HATA] Branch degisimi basarisiz. & pause & exit /b)
)

echo.
echo [1/6] Uzak durum bilgisi cekiliyor (fetch - DEGİSİKLİK UYGULANMAZ)...
git fetch origin || echo [UYARI] fetch basarisiz, devam ediliyor.

echo [2/6] Pull/Rebase ATLANDI (lokal -> uzak yazilacak)...
REM (Önceden burada pull --rebase vardi; artik yok.)

REM ==== VERSION.TXT kontrol ====
if not exist "version.txt" (echo 1.0.0>version.txt)

for /f "usebackq tokens=1-3 delims=." %%a in ("version.txt") do (
  set MAJOR=%%a
  set MINOR=%%b
  set PATCH=%%c
)

if not defined MAJOR set MAJOR=1
if not defined MINOR set MINOR=0
if not defined PATCH set PATCH=0

echo.
echo ===============================
echo  Versiyon Sec (su an: %MAJOR%.%MINOR%.%PATCH%)
echo ===============================
echo  1) Patch  (kucuk degisiklik / bugfix)
echo  2) Minor  (ozellik / yeni level eklendi)
echo  3) Major  (buyuk degisiklik / uyumsuzluk)
set /p CHOICE="Secim: "

if "%CHOICE%"=="1" set /a PATCH+=1
if "%CHOICE%"=="2" (set /a MINOR+=1 & set PATCH=0)
if "%CHOICE%"=="3" (set /a MAJOR+=1 & set MINOR=0 & set PATCH=0)

set "NEWVER=%MAJOR%.%MINOR%.%PATCH%"

> version.txt echo %NEWVER%
echo [3/6] Versiyon yukseltildi: %NEWVER%

REM --- Unity Build Settings (bundleVersion) ile esitle ---
set "PSF=ProjectSettings\ProjectSettings.asset"
if exist "%PSF%" (
  REM bundleVersion: X.Y.Z satirini NEWVER ile degistir
  powershell -NoProfile -Command ^
    "(Get-Content '%PSF%') -replace '^(bundleVersion:\s*).*$','$1%NEWVER%' | Set-Content '%PSF%'"
  
  REM Android versionCode hesapla (MAJOR*10000 + MINOR*100 + PATCH)
  set /a ANDROID_CODE=%MAJOR%*10000 + %MINOR%*100 + %PATCH%
  powershell -NoProfile -Command ^
    "(Get-Content '%PSF%') -replace '^(AndroidBundleVersionCode:\s*)\d+','$1%ANDROID_CODE%' | Set-Content '%PSF%'"

  REM iOS build number (numerik olmalidir)
  powershell -NoProfile -Command ^
    "(Get-Content '%PSF%') -replace '^(iPhoneBuildNumber:\s*)\d+','$1%ANDROID_CODE%' | Set-Content '%PSF%'"

  echo [3.5/6] Unity bundleVersion ve build numaralari guncellendi: %NEWVER% / %ANDROID_CODE%
) else (
  echo [UYARI] ProjectSettings.asset bulunamadi: %PSF%
)

echo [4/6] Degisiklikler ekleniyor...
git add -A
git diff --cached --quiet && (
  echo [BILGI] Commitlenecek bir degisiklik yok.
) || (
  git commit -m "ColorCemberRun: bump version to %NEWVER% + sync"
)

echo [5/6] GitHub'a push ediliyor (LOKAL -> UZAK, force-with-lease)...
git push -u --force-with-lease origin %MAIN_BRANCH% || (
  echo [HATA] Push basarisiz. 
  pause & exit /b
)

echo.
echo ===============================
echo  ✓ Islem tamam: %NEWVER%  (Yerel -> GitHub yazildi)
echo ===============================
popd
pause