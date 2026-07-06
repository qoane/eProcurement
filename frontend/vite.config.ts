import { existsSync } from 'node:fs';
import { join } from 'node:path';
import { execFileSync } from 'node:child_process';
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const requiredVendorAssets = [
  'node_modules/bootstrap/dist/css/bootstrap.min.css',
  'node_modules/bootstrap-icons/font/bootstrap-icons.css',
  'node_modules/overlayscrollbars/package.json',
  'node_modules/admin-lte/dist/css/adminlte.min.css',
  'node_modules/simple-datatables/dist/style.css',
];

function ensureFrontendDependencies() {
  const missingVendorAsset = requiredVendorAssets.find(
    (assetPath) => !existsSync(join(__dirname, assetPath)),
  );

  if (!missingVendorAsset) return;

  console.warn(
    `Missing frontend dependency asset (${missingVendorAsset}); running npm install before Vite starts.`,
  );

  execFileSync('npm', ['install', '--include=dev', '--no-audit', '--no-fund'], {
    cwd: __dirname,
    stdio: 'inherit',
    shell: process.platform === 'win32',
  });
}

ensureFrontendDependencies();

export default defineConfig({ plugins: [react()], server: { port: 5173 } });
