﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatMoa_DataBaseServer;

namespace MainSystem
{
    public partial class SignupForm : Form
    {
        private DCM dcm; // 데이터베이스 클라이언트 모듈

        // 비밀번호 찾기 질문 목록 (인덱스로 관리)
        private string[] passwordQuestions = new string[]
        {
            "가장 좋아하는 음식은?",
            "첫 번째 애완동물의 이름은?",
            "어머니의 성함은?",
            "출신 초등학교는?",
            "가장 좋아하는 색깔은?"
        };

        public SignupForm()
        {
            InitializeComponent();
            dcm = new DCM(); // DCM 인스턴스 생성
            InitializeForm();
        }

        private void InitializeForm()
        {
            // 유효성 검사 라벨 초기화
            label1.Text = "";  // ID 유효성
            label8.Text = "";  // PW 유효성
            label9.Text = "";  // PW 확인 유효성
            label10.Text = ""; // 질문 유효성
            label11.Text = ""; // 답변 유효성
            label12.Text = ""; // 이름 유효성
            label13.Text = ""; // 닉네임 유효성

            // 비밀번호 질문을 textBox2에 표시 (첫 번째 질문으로 기본 설정)
            textBox2.Text = passwordQuestions[0];
            textBox2.Tag = 0; // 질문 인덱스 저장
            textBox2.ReadOnly = true;
            textBox2.BackColor = SystemColors.Control;

            // 질문 선택 버튼 추가 (textBox2 옆에)
            Button btnSelectQuestion = new Button();
            btnSelectQuestion.Text = "▼";
            btnSelectQuestion.Size = new Size(25, textBox2.Height);
            btnSelectQuestion.Location = new Point(textBox2.Right - 25, textBox2.Top);
            btnSelectQuestion.Click += BtnSelectQuestion_Click;
            this.Controls.Add(btnSelectQuestion);
            btnSelectQuestion.BringToFront();
        }

        private void BtnSelectQuestion_Click(object sender, EventArgs e)
        {
            // 간단한 질문 선택 다이얼로그
            using (Form questionForm = new Form())
            {
                questionForm.Text = "비밀번호 찾기 질문 선택";
                questionForm.Size = new Size(350, 200);
                questionForm.StartPosition = FormStartPosition.CenterParent;

                ListBox listBox = new ListBox();
                listBox.Dock = DockStyle.Top;
                listBox.Height = 120;
                listBox.Items.AddRange(passwordQuestions);
                listBox.SelectedIndex = (int)(textBox2.Tag ?? 0);

                Button btnOK = new Button();
                btnOK.Text = "확인";
                btnOK.DialogResult = DialogResult.OK;
                btnOK.Location = new Point(135, 130);

                questionForm.Controls.Add(listBox);
                questionForm.Controls.Add(btnOK);

                if (questionForm.ShowDialog() == DialogResult.OK && listBox.SelectedIndex >= 0)
                {
                    textBox2.Text = passwordQuestions[listBox.SelectedIndex];
                    textBox2.Tag = listBox.SelectedIndex;
                }
            }
        }

        private void SignupForm_Load(object sender, EventArgs e)
        {
            // 폼 로드 시 초기화 작업
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // 입력값 가져오기
                string newID = txtNewID.Text.Trim();
                string newPW = txtNewPW.Text.Trim();
                string confirmPW = textBox1.Text.Trim(); // 비밀번호 확인
                string psAnswer = textBox3.Text.Trim();  // 비밀번호 답변
                string newName = txtName.Text.Trim();
                string newNick = txtNick.Text.Trim();
                int psQuestionIndex = (int)(textBox2.Tag ?? 0);

                // 입력값 검증
                bool isValid = true;

                // ID 검증
                if (string.IsNullOrEmpty(newID))
                {
                    label1.Text = "필수 입력";
                    label1.ForeColor = Color.Red;
                    isValid = false;
                }
                else if (newID.Length < 4)
                {
                    label1.Text = "4자 이상";
                    label1.ForeColor = Color.Red;
                    isValid = false;
                }
                else
                {
                    label1.Text = "✓";
                    label1.ForeColor = Color.Green;
                }

                // 비밀번호 검증
                if (string.IsNullOrEmpty(newPW))
                {
                    label8.Text = "필수 입력";
                    label8.ForeColor = Color.Red;
                    isValid = false;
                }
                else if (newPW.Length < 6)
                {
                    label8.Text = "6자 이상";
                    label8.ForeColor = Color.Red;
                    isValid = false;
                }
                else
                {
                    label8.Text = "✓";
                    label8.ForeColor = Color.Green;
                }

                // 비밀번호 확인 검증
                if (newPW != confirmPW)
                {
                    label9.Text = "불일치";
                    label9.ForeColor = Color.Red;
                    isValid = false;
                }
                else if (!string.IsNullOrEmpty(confirmPW))
                {
                    label9.Text = "✓";
                    label9.ForeColor = Color.Green;
                }

                // 비밀번호 답변 검증
                if (string.IsNullOrEmpty(psAnswer))
                {
                    label11.Text = "필수 입력";
                    label11.ForeColor = Color.Red;
                    isValid = false;
                }
                else
                {
                    label11.Text = "✓";
                    label11.ForeColor = Color.Green;
                }

                // 이름 검증
                if (string.IsNullOrEmpty(newName))
                {
                    label12.Text = "필수 입력";
                    label12.ForeColor = Color.Red;
                    isValid = false;
                }
                else
                {
                    label12.Text = "✓";
                    label12.ForeColor = Color.Green;
                }

                // 닉네임 검증
                if (string.IsNullOrEmpty(newNick))
                {
                    label13.Text = "필수 입력";
                    label13.ForeColor = Color.Red;
                    isValid = false;
                }
                else
                {
                    label13.Text = "✓";
                    label13.ForeColor = Color.Green;
                }

                if (!isValid)
                {
                    MessageBox.Show("입력 정보를 확인해주세요.", "입력 오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 버튼 비활성화 (중복 클릭 방지)
                btnRegister.Enabled = false;
                btnRegister.Text = "Processing...";

                // 서버에 회원가입 요청
                // opcode 1: register(회원가입)
                // need items = {id, ps, ps_question_index, ps_question_answer, name, nickname}
                List<string> items = new List<string>
                {
                    newID,
                    newPW,
                    psQuestionIndex.ToString(),
                    psAnswer,
                    newName,
                    newNick
                };

                // 서버에 요청 전송
                var result = await dcm.db_request_data(1, items);

                // 결과 처리
                if (result.Key) // 서버 응답을 받은 경우
                {
                    int key = result.Value.Item1;
                    List<int> indexes = result.Value.Item2;

                    // 응답 데이터 확인
                    if (indexes.Count > 0)
                    {
                        // 서버로부터 받은 응답 확인
                        string response = GetResponseData(key, indexes.Last());

                        if (response == "1")
                        {
                            MessageBox.Show("회원가입이 성공적으로 완료되었습니다!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // DCM의 received_data 정리
                            ClearReceivedData(key);

                            // 폼 닫기
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else if (response == "0")
                        {
                            MessageBox.Show("이미 존재하는 아이디입니다.", "Registration Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            label1.Text = "중복된 ID";
                            label1.ForeColor = Color.Red;

                            // DCM의 received_data 정리
                            ClearReceivedData(key);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("서버 연결에 실패했습니다. 잠시 후 다시 시도해주세요.", "Connection Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류가 발생했습니다: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 버튼 다시 활성화
                btnRegister.Enabled = true;
                btnRegister.Text = "Register";
            }
        }

        // DCM의 received_data에서 데이터를 가져오는 헬퍼 메서드
        private string GetResponseData(int key, int index)
        {
            try
            {
                var dcmType = dcm.GetType();
                var receivedDataField = dcmType.GetField("received_data",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (receivedDataField != null)
                {
                    var receivedData = receivedDataField.GetValue(dcm) as Dictionary<int, List<string>>;
                    if (receivedData != null && receivedData.ContainsKey(key))
                    {
                        var dataList = receivedData[key];
                        if (index < dataList.Count)
                        {
                            return dataList[index];
                        }
                    }
                }
            }
            catch { }

            return "0"; // 기본값: 실패
        }

        // DCM의 received_data를 정리하는 헬퍼 메서드
        private void ClearReceivedData(int key)
        {
            try
            {
                var dcmType = dcm.GetType();
                var clearMethod = dcmType.GetMethod("Clear_receive_data",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (clearMethod != null)
                {
                    clearMethod.Invoke(dcm, new object[] { key });
                }
            }
            catch { }
        }

        private void txtNewID_TextChanged(object sender, EventArgs e)
        {
            // 실시간 ID 유효성 검사
            if (string.IsNullOrEmpty(txtNewID.Text))
            {
                label1.Text = "";
            }
            else if (txtNewID.Text.Length < 4)
            {
                label1.Text = "4자 이상 필요";
                label1.ForeColor = Color.Orange;
            }
            else
            {
                label1.Text = "사용 가능";
                label1.ForeColor = Color.Green;
            }
        }

        private void txtNewPW_TextChanged(object sender, EventArgs e)
        {
            // 실시간 비밀번호 유효성 검사
            if (string.IsNullOrEmpty(txtNewPW.Text))
            {
                label8.Text = "";
            }
            else if (txtNewPW.Text.Length < 6)
            {
                label8.Text = "6자 이상 필요";
                label8.ForeColor = Color.Orange;
            }
            else
            {
                label8.Text = "사용 가능";
                label8.ForeColor = Color.Green;
            }

            // 비밀번호 확인 필드와 비교
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                if (txtNewPW.Text != textBox1.Text)
                {
                    label9.Text = "비밀번호 불일치";
                    label9.ForeColor = Color.Red;
                }
                else
                {
                    label9.Text = "일치";
                    label9.ForeColor = Color.Green;
                }
            }
        }
    }
}