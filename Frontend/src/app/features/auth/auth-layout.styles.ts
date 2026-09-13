export const authStyles = `
  .auth-page {
    min-height: 100vh;
    display: grid;
    grid-template-columns: minmax(420px, .92fr) minmax(520px, 1.08fr);
    background: #f5f7fa;
  }
  .auth-art {
    position: relative;
    isolation: isolate;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    min-height: 100vh;
    padding: clamp(34px, 5vw, 68px);
    overflow: hidden;
    background: linear-gradient(145deg, #111b31 0%, #1a2c58 62%, #244aa9 130%);
    color: #fff;
  }
  .auth-art::before {
    position: absolute;
    z-index: -1;
    inset: 0;
    opacity: .19;
    background-image:
      linear-gradient(rgba(255,255,255,.08) 1px, transparent 1px),
      linear-gradient(90deg, rgba(255,255,255,.08) 1px, transparent 1px);
    background-size: 44px 44px;
    mask-image: linear-gradient(to bottom right, #000, transparent 78%);
    content: '';
  }
  .auth-art::after {
    position: absolute;
    z-index: -1;
    width: 620px;
    height: 620px;
    right: -310px;
    bottom: -330px;
    background: radial-gradient(circle, rgba(74,120,236,.62), transparent 66%);
    border-radius: 50%;
    content: '';
  }
  .auth-brand { position: relative; z-index: 1; display: flex; align-items: center; gap: 12px; }
  .auth-logo {
    display: grid;
    place-items: center;
    width: 44px;
    height: 44px;
    background: #3766dd;
    color: #fff;
    border: 1px solid rgba(255,255,255,.2);
    border-radius: 11px;
    box-shadow: 0 10px 25px rgba(7,22,59,.22);
    font-family: Manrope, sans-serif;
    font-size: 1.12rem;
    font-weight: 800;
  }
  .auth-brand strong { display: block; color: #fff; font-family: Manrope, sans-serif; font-size: 1.04rem; letter-spacing: -.02em; }
  .auth-brand span { display: block; margin-top: 2px; color: #9dabc3; font-size: .69rem; }
  .auth-copy { position: relative; z-index: 1; max-width: 650px; margin: 72px 0; }
  .auth-copy h1 { max-width: 620px; color: #fff; font-size: clamp(2.1rem, 4vw, 4rem); line-height: 1.07; letter-spacing: -.055em; }
  .auth-copy p { max-width: 570px; margin: 19px 0 0; color: #bdc9df; font-size: .98rem; line-height: 1.75; }
  .auth-copy > span {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    margin-bottom: 17px;
    color: #9fbbff !important;
    font-size: .68rem !important;
    font-weight: 700 !important;
    letter-spacing: .12em !important;
  }
  .auth-copy > span::before { width: 18px; height: 1px; background: currentColor; content: ''; }
  .auth-features { position: relative; z-index: 1; display: flex; flex-wrap: wrap; gap: 8px; }
  .auth-chip {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 8px 11px;
    background: rgba(255,255,255,.055);
    color: #cbd5e6;
    border: 1px solid rgba(255,255,255,.1);
    border-radius: 8px;
    font-size: .69rem;
    font-weight: 600;
    backdrop-filter: blur(8px);
  }
  .auth-chip::before { width: 6px; height: 6px; background: #76a0ff; border-radius: 50%; content: ''; box-shadow: 0 0 0 3px rgba(118,160,255,.12); }
  .auth-main { display: grid; place-items: center; padding: 38px; }
  .auth-card {
    width: min(500px, 100%);
    padding: 34px;
    background: #fff;
    border: 1px solid #e1e5eb;
    border-radius: 17px;
    box-shadow: 0 20px 60px rgba(16,24,40,.07);
  }
  .auth-card h2 { font-size: 1.65rem; letter-spacing: -.035em; }
  .auth-card > p { margin: 8px 0 25px; color: #667085; font-size: .89rem; }
  .auth-card .btn { width: 100%; }
  .auth-link { color: #2855d9; font-weight: 700; }
  .auth-link:hover { color: #1d43ae; text-decoration: underline; text-underline-offset: 3px; }
  .auth-footer { margin-top: 20px; color: #667085; font-size: .81rem; text-align: center; }
  .password-wrap { position: relative; }
  .password-wrap .input { padding-right: 70px; }
  .password-toggle { position: absolute; top: 50%; right: 8px; padding: 6px 7px; background: transparent; color: #59667a; border: 0; border-radius: 6px; font-size: .7rem; font-weight: 700; transform: translateY(-50%); }
  .password-toggle:hover { background: #f3f5f8; }
  .checks { display: flex; align-items: center; justify-content: space-between; gap: 12px; font-size: .78rem; }
  .checks label { display: flex; align-items: center; gap: 8px; color: #475467; }
  .index-check { display: flex; gap: 8px; }
  .index-check .input { flex: 1; }
  .index-check .btn { width: auto; white-space: nowrap; }
  .master-box { padding: 11px 12px; background: #eef3ff; color: #27499f; border: 1px solid #d5e1ff; border-radius: 9px; font-size: .78rem; line-height: 1.5; }

  @media (max-width: 980px) {
    .auth-page { grid-template-columns: 1fr; }
    .auth-art { min-height: 310px; padding: 28px; }
    .auth-copy { margin: 60px 0 10px; }
    .auth-copy h1 { max-width: 650px; font-size: clamp(2rem, 7vw, 3rem); }
    .auth-copy p { max-width: 680px; }
    .auth-main { padding: 28px 20px; }
  }
  @media (max-width: 560px) {
    .auth-art { min-height: 280px; padding: 22px; }
    .auth-copy { margin-top: 50px; }
    .auth-copy h1 { font-size: 2rem; }
    .auth-copy p { font-size: .88rem; line-height: 1.6; }
    .auth-features { display: none; }
    .auth-main { padding: 18px 12px 28px; align-items: start; }
    .auth-card { padding: 23px 19px; border-radius: 14px; }
    .checks { align-items: flex-start; }
    .index-check { display: grid; }
    .index-check .btn { width: 100%; }
  }
`;
