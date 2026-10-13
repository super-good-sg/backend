import { test } from 'node:test';
import assert from 'node:assert/strict';
import { spawnSync } from 'node:child_process';

// Runs the real CLI so the config's `extends` rules (header length, type case) are covered too.
const ok = (msg) => spawnSync('npx', ['--no', 'commitlint'], { input: msg, encoding: 'utf8' }).status === 0;

test('accepts good headers', () => {
  for (const m of ['feat(SIE-85): add pr template', 'fix(SIE-85,86): handle login error', 'docs(SIE-1): add 2fa notes'])
    assert.equal(ok(m), true, m);
});

test('rejects bad headers', () => {
  for (const m of [
    'fix: login bug',
    'fix(login): handle error',
    'feat(SIE-85|backend): add x',
    'feat(sie-85): add x',
    'feat(SIE-85): Add x',
    'wip(SIE-85): add x',
    `feat(SIE-85): ${'x'.repeat(100)}`,
  ])
    assert.equal(ok(m), false, m);
});
