// Rules: docs/CONVENTIONS.md. Header: type(SIE-85): lowercase message.
const sie = {
  rules: {
    'sie-scope-format': ({ scope }) => [
      /^SIE-\d+(,\d+)*$/.test(scope ?? ''),
      'scope must be the Linear ticket, SIE-NNN or SIE-NNN,NNN (several tickets), see docs/CONVENTIONS.md',
    ],
    'sie-subject-lowercase': ({ subject }) => [
      /^[a-z]/.test(subject ?? ''),
      'the commit message must start with a lowercase letter a-z',
    ],
  },
};

export default {
  extends: ['@commitlint/config-conventional'],
  plugins: [sie],
  rules: {
    'type-enum': [
      2,
      'always',
      ['build', 'chore', 'ci', 'docs', 'feat', 'fix', 'hotfix', 'perf', 'refactor', 'revert', 'style', 'test'],
    ],
    'scope-empty': [2, 'never'],
    'sie-scope-format': [2, 'always'],
    'sie-subject-lowercase': [2, 'always'],
  },
};
