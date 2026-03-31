@echo off
echo === Starting Git Setup ===
git init
echo === Git initialized ===
git remote add origin https://github.com/shefa2024/kraen-proekt.git
echo === Remote added ===
git add -A
echo === Files staged ===
git commit -m "Initial commit - LearnConnect platform"
echo === Committed ===
git branch -M main
echo === Branch set to main ===
git push -u origin main --force
echo === Push complete ===
echo === ALL DONE ===
pause
